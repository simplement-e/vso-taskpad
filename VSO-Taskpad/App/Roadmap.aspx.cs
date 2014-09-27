using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VSO_Taskpad.App
{
    public partial class Roadmap : ProjectPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var prj = GetProject();
            if (prj != null)
                lblTitre.Text = string.Format("roadmap for {0}", prj.Name);

        }
    }
}