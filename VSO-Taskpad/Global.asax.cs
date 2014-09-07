using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using SimplementE.VisualStudio.TaskPad.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

[assembly: PreApplicationStartMethod(
typeof(VSO_Taskpad.Global), "PreStartupApplication")]

namespace VSO_Taskpad
{
    public class Global : System.Web.HttpApplication
    {
        public static void PreStartupApplication()
        {
            DynamicModuleUtility.RegisterModule(typeof(ExternalAuthService));
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(App_Start.WebApiConfig.Register);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}