using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VSO_Taskpad
{
    public partial class OAuthError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string kind = Request["kind"];
            if (string.IsNullOrEmpty(kind))
                kind = "generic";
            else
                kind = kind.ToLower();

            switch(kind)
            {
                case "access_denied":
                    mvError.SetActiveView(vwNoGrant);
                    break;
                default:
                    mvError.SetActiveView(vwGeneric);
                    break;
            }

        }
    }
}