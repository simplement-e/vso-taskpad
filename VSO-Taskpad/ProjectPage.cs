using SimplementE.VisualStudio.TaskPad.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace VSO_Taskpad
{
    public class ProjectPage : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            var prj = RouteData.Values["project"];
            if (prj == null)
                Response.Redirect("~/", false);
            base.OnLoad(e);
        }

        protected VsoProject GetProject()
        {
            var prj = RouteData.Values["project"];
            if (prj == null)
                return null;
            string name = prj.ToString();
            Dictionary<string, VsoProject> prjs = Session["projects"] as Dictionary<string, VsoProject>;
            if (prjs == null)
            {

            }
            VsoProject p;
            if (!prjs.TryGetValue(name, out p))
                return null;
            return p;
        }
    }
}