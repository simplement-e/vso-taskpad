using SimplementE.VisualStudio.TaskPad.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VSO_Taskpad
{
    public partial class HeaderControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.User.Identity.IsAuthenticated)
            {
                this.Visible= false;
                return;
            }

            lnkUserName.Text = Page.User.Identity.Name;

            var prjs = UserSession.AllProjects;

            prjs = (from z in prjs
                    orderby z.Index descending
                    select z).ToArray();

            var selected = (Page is ProjectPage ? (Page as ProjectPage).GetProject() : null);
            List<ProjectItem> items = new List<ProjectItem>();
            
            foreach(var p in prjs)
            {
                var it = new ProjectItem() { css = "", id = p.Guid.ToString("N"), name = p.Name };
                if (selected != null && p.Guid.Equals(selected.Guid))
                    it.css = "selected";
                items.Add(it);
            }
            rptProjects.DataSource = items;
            rptProjects.DataBind();

            if(!(Page is ProjectPage))
            {
                lnkHome.CssClass = "selected";
            }
        }

        private class ProjectItem
        {
            public string name { get; set; }
            public string id { get; set; }
            public string css { get; set; }
        }
    }
}