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

            return new WorkItem[0];
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
