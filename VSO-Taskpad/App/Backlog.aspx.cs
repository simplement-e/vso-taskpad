using SimplementE.TaskPad.Business;
using SimplementE.TaskPad.Business.VsoApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VSO_Taskpad.App
{
    public partial class Backlog : ProjectPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var p = GetProject();
            var acc = UserSession.GetAccountFromProject(p);
            var br = WorkItems.GetBacklog(acc.Name, UserSession.Credentials, p.Name);
            rptProjects.DataSource = br;
            rptProjects.DataBind();
        }
    }
}