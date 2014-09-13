using SimplementE.TaskPad.Business;
using SimplementE.TaskPad.Business.VsoApi;
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
            if (!IsPostBack)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    mvAuth.SetActiveView(vwNotAuth);
                    Title = "vso-taskpad : work items made easy";
                }
                else if (UserSession.Credentials == null)
                {
                    mvAuth.SetActiveView(vwAuthNoVso);
                    Title = "vso-taskpad - not connected to vso ";

                }
                else
                {
                    mvAuth.SetActiveView(vwAuth);
                    Title = "vso-taskpad : work items made easy";
                }
            }
        }
    }
}