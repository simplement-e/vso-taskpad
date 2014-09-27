using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SimplementE.TaskPad.Business.VsoApi
{
    public static class WorkItems
    {
        [Serializable]
        private class QueryResult
        {
            public List<WorkItemRelationIdData> workItems { get; set; }
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
        public class WorkItemRelationData
        {
            public WorkItemRelationIdData target { get; set; }
        }

        [Serializable]
        public class WorkItemRelationIdData
        {
            public int id { get; set; }
            public string url { get; set; }
        }

        [Serializable]
        private class WorkItemData
        {
            public int id { get; set; }
            public int rev { get; set; }
            public FieldData fields { get; set; }
        }

        [JsonObject]
        private class FieldData
        {
            [JsonProperty("System.Title")]
            public string Title { get; set; }
            [JsonProperty("System.WorkItemType")]
            public string WorkItemType { get; set; }

            [JsonProperty("System.IterationPath")]
            public string IterationPath { get; set; }
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
            public string query { get; set; }
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
            var qry = new WiqlQuery() { query = queryString };


            var url = "https://{account}.visualstudio.com/defaultcollection/"+ Uri.EscapeUriString(project)+"/_apis/wit/wiql?api-version=1.0-preview.2";

            var r = VsoWebServiceHelper.Raw(account, cred,
                url,
                "POST", JsonConvert.SerializeObject(qry));

            QueryResult res = JsonConvert.DeserializeObject<QueryResult>(r);

            List<WorkItem> ret = new List<WorkItem>();

            string fields = "system.id,system.title,system.state";

            while (res.workItems.Count > 0)
            {
                StringBuilder blr = new StringBuilder();
                for (int i = 0; i < 200 && res.workItems.Count > 0; i++)
                {
                    if (i != 0)
                        blr.Append(",");
                    blr.Append(res.workItems[0].id);
                    res.workItems.RemoveAt(0);
                }

                r = VsoWebServiceHelper.Raw(account, cred,
                    "https://{account}.visualstudio.com/defaultcollection/_apis/wit/workitems?ids="
                    + blr.ToString() + " &api-version=1.0-preview.2",
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
            if (string.IsNullOrEmpty(it.fields.WorkItemType))
                return null;

            WorkItem wk = null;
            switch (it.fields.WorkItemType.ToLower())
            {
                case "product backlog item":
                    wk = new ProductBacklogItem();
                    break;
                case "bug":
                    wk = new BugItem();
                    break;
                case "feature":
                    wk = new FeatureItem();
                    break;
            }

            wk.label = it.fields.Title;
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

    public class FeatureItem : WorkItem
    {

    }

}
