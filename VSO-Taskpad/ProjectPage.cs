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

        protected internal Project GetProject()
        {
            var prj = RouteData.Values["project"];
            if (prj == null)
                return null;
            string name = prj.ToString();
            var prjs = UserSession.AllProjects;
            if (prjs == null)
            {
                return null;
            }
            return (from z in prjs
                        where z.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                        select z).FirstOrDefault();
        }
    }
}