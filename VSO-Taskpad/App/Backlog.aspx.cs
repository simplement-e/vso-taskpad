using SimplementE.VisualStudio.TaskPad.Business;
using SimplementE.VisualStudio.TaskPad.Business.VsoApi;
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
            var prj = GetProject();
            if (prj != null)
                lblProject.Text = prj.Name;
            else
                Response.Redirect("~/", false);

            VsoWebServiceCredentials vso = Session["auth"] as VsoWebServiceCredentials;

            var items = WorkItems.GetBacklog(vso, prj.Name);
            rptProjects.DataSource = items;
            rptProjects.DataBind();
        }
    }
}