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
        public static WorkItem[] GetBacklog(VsoWebServiceCredentials cred, string project)
        {
            StringBuilder blr = new StringBuilder();
            blr.Append("select [System.Id]");
            blr.Append(" From WorkItem where [System.WorkItemType] IN GROUP 'Microsoft.RequirementCategory'");
            blr.Append(" and [System.State] IN ('New','Approved','Committed')");
            var qry = new WiqlQuery() { wiql = blr.ToString() };

            var r = VsoWebServiceHelper.Raw(cred,
                "https://{account}.visualstudio.com/defaultcollection/_apis/wit/queryresults?api-version=1.0-preview",
                "POST", JsonConvert.SerializeObject(qry));

            QueryResult res = JsonConvert.DeserializeObject<QueryResult>(r);

            List<WorkItem> ret = new List<WorkItem>();

            string fields = "System.Id";

            while (res.results.Count > 0)
            {
                blr = new StringBuilder();
                for (int i = 0; i < 200 && res.results.Count > 0; i++)
                {
                    if (i != 0)
                        blr.Append(",");
                    blr.Append(res.results[0].sourceId);
                    res.results.RemoveAt(0);
                }

                r = VsoWebServiceHelper.Raw(cred,
                    "https://{account}.visualstudio.com/defaultcollection/_apis/wit/workitems?ids="
                    + blr.ToString() + " &api-version=1.0-preview",
    "GET");
                //+ "&fields=" + fields 

                GetItemsResult gi = JsonConvert.DeserializeObject<GetItemsResult>(r);

            }

            return ret.ToArray();
        }
    }

    public class WorkItem
    {
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
