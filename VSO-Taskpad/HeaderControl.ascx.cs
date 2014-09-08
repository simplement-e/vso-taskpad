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
        }
    }
}