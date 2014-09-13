using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VSO_Taskpad.App
{
    public partial class ProjectHeaderControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var pg = (Page as ProjectPage);
            if (pg == null)
                throw new ApplicationException("Page must be a ProjectPage");

            var prj = pg.GetProject();

            lnkBugsProject.NavigateUrl = string.Format("~/app/{0}/bugs", prj.Name);
            lnkHomeProject.NavigateUrl = string.Format("~/app/{0}", prj.Name);

            if (Page is Bugs)
                lnkBugsProject.CssClass = "selected";
            else if (Page is Default)
                lnkHomeProject.CssClass = "selected";
        }
    }
}