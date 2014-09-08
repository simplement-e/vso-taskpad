using SimplementE.VisualStudio.TaskPad.Business;
using SimplementE.VisualStudio.TaskPad.Business.VsoApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VSO_Taskpad
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                //if (Session["auth"] != null)
                //{
                //    VsoOauthCredentials cred = Session["auth"] as VsoOauthCredentials;
                //    var r = WorkItems.GetBacklog(cred, "Equihira");
                //    rptProjects.DataSource = r;
                //    rptProjects.DataBind();
                //}
                //else
                //{
                //    Response.Redirect("~/oauth/vstudio/start/");
                //}

                if(!User.Identity.IsAuthenticated)
                {
                    mvAuth.SetActiveView(vwNotAuth);
                }
                else if (Session["auth"]==null)
                {
                    mvAuth.SetActiveView(vwAuthNoVso);
                }
                else
                {
                    mvAuth.SetActiveView(vwAuth);
                }
            }
        }
    }
}