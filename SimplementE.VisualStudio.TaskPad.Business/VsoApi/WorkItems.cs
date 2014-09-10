using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SimplementE.VisualStudio.TaskPad.Business.VsoApi
{
    public static class WorkItems
    {
        [Serializable]
        private class QueryResult
        {
            public List<QueryResultItem> results { get; set; }
        }

        [Serializable]
        private class QueryResultItem
        {
            public int sourceId { get; set; }
        }

        [Serializable]
        private class GetItemsResult
        {
            public WorkItemData[] value { get; set; }
        }

        [Serializable]
        private class WorkItemData
        {
            public int id { get; set; }
            public int rev { get; set; }
            public FieldData[] fields { get; set; }
        }

        [Serializable]
        private class FieldData
        {
            public string value { get; set; }
            public FieldDesc field { get; set; }
        }

        [Serializable]
        private class FieldDesc
        {
            public string id { get; set; }
            public string refName { get; set; }
        }

        [Serializable]
        private class WiqlQuery
        {
            public string wiql { get; set; }
        }

        public static WorkItem[] GetBacklog(string account, VsoWebServiceCredentials cred, string project = null)
        {
            StringBuilder blr = new StringBuilder();
            blr.Append("select [System.Id]");
            blr.Append(" From WorkItem where [System.WorkItemType] IN GROUP 'Microsoft.RequirementCategory'");
            blr.Append(" and [System.State] IN ('New','Approved','Committed')");
            if (!string.IsNullOrEmpty(project))
                blr.Append(" and [System.TeamProject] = @project");
            return GetWorkItemsFromQuery(account, cred, blr.ToString(), project);
        }

        private static WorkItem[] GetWorkItemsFromQuery(string account, VsoWebServiceCredentials cred, string queryString, string project = null)
        {
            var qry = new WiqlQuery() { wiql = queryString };


            var url = "https://{account}.visualstudio.com/defaultcollection/_apis/wit/queryresults?api-version=1.0-preview";
            if(!string.IsNullOrEmpty(project))
                url += "&@project=" + Uri.EscapeUriString(project);

            var r = VsoWebServiceHelper.Raw(account, cred,
                url,
                "POST", JsonConvert.SerializeObject(qry));

            QueryResult res = JsonConvert.DeserializeObject<QueryResult>(r);

            List<WorkItem> ret = new List<WorkItem>();

            string fields = "System.Id";

            while (res.results.Count > 0)
            {
                StringBuilder blr = new StringBuilder();
                for (int i = 0; i < 200 && res.results.Count > 0; i++)
                {
                    if (i != 0)
                        blr.Append(",");
                    blr.Append(res.results[0].sourceId);
                    res.results.RemoveAt(0);
                }

                r = VsoWebServiceHelper.Raw(account, cred,
                    "https://{account}.visualstudio.com/defaultcollection/_apis/wit/workitems?ids="
                    + blr.ToString() + " &api-version=1.0-preview",
    "GET");
                //+ "&fields=" + fields 

                GetItemsResult gi = JsonConvert.DeserializeObject<GetItemsResult>(r);
                foreach (var it in gi.value)
                {
                    var newit = ConvertToWorkItem(it);
                    if (newit != null)
                        ret.Add(newit);
                }
            }

            return ret.ToArray();
        }

        private static WorkItem ConvertToWorkItem(WorkItemData it)
        {
            string typ = ((from z in it.fields
                       where z.field.refName.Equals("System.WorkItemType", StringComparison.InvariantCultureIgnoreCase)
                       select z).FirstOrDefault().value);
            if (string.IsNullOrEmpty(typ))
                return null;

            WorkItem wk = null;

            switch(typ.ToLower())
            {
                case "product backlog item":
                    wk = new ProductBacklogItem();
                    break;
                case "bug":
                    wk = new BugItem();
                    break;
            }
            if (wk != null)
            {
                typ = ((from z in it.fields
                        where z.field.refName.Equals("System.Title", StringComparison.InvariantCultureIgnoreCase)
                        select z).FirstOrDefault().value);
                wk.label = typ;
                typ = ((from z in it.fields
                        where z.field.refName.Equals("System.IterationPath", StringComparison.InvariantCultureIgnoreCase)
                        select z).FirstOrDefault().value);
                wk.iterationPath = typ;
            }
            return wk;
        }
    }

    public class WorkItem
    {
        public string iterationPath { get; set; }
        public string label { get; set; }
        public int id { get; set; }
        public string type { get; set; }
    }

    public class ProductBacklogItem : WorkItem
    {

    }

    public class BugItem : WorkItem
    {

    }

}
