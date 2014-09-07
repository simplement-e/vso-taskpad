using SimplementE.VisualStudio.TaskPad.Business;
using SimplementE.VisualStudio.TaskPad.Business.VsoApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace VSO_Taskpad.app
{
    [RoutePrefix("app/api/workitems")]
    public class WorkItemsController : ApiController
    {
        [Route("productbacklog")]
        public IEnumerable<WorkItem> GetProductBacklog()
        {
            var cred = new VsoBasicCredentials();
            var r = WorkItems.GetBacklog(cred, "Equihira");

            return r;
        }

        [Route("iterations")]
        public IEnumerable<ProjectIteration> GetIterations()
        {
            var cred = new VsoBasicCredentials();
            Projects.GetProjects(cred);
            return null;
        }
    }
}