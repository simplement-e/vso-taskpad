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
                var cred = new VsoBasicCredentials();
                var r = WorkItems.GetBacklog(cred, "");
                rptProjects.DataSource = r;
                rptProjects.DataBind();
            }
        }
    }
}