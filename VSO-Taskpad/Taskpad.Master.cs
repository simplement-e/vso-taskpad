using SimplementE.VisualStudio.TaskPad.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VSO_Taskpad
{
    public partial class Taskpad : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserSession.Credentials == null)
            {
                string url = Page.Request.RawUrl.ToLower();
                url = VirtualPathUtility.ToAppRelative(url);
                if (url.StartsWith("~/account/"))
                    return;

                if (Page is _default)
                    return;

                if (Page is OAuthError)
                    return;

                Response.Redirect("~/");
            }

        }
    }
}