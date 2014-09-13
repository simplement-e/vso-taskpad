using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using SimplementE.TaskPad.Business;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Http;
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
            DynamicModuleUtility.RegisterModule(typeof(AuthService));
            DynamicModuleUtility.RegisterModule(typeof(VSO_Taskpad.hooks.VsoModule));
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(App_Start.WebApiConfig.Register);
            App_Start.RouteConfig.Register();

        }

        protected void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var url = Context.Request.Url;
            if (!url.Scheme.Equals("https"))
            {
                if (url.Host.Equals("vso-taskpad.azurewebsites.net", StringComparison.InvariantCultureIgnoreCase))
                {
                    url = new Uri("https" + url.ToString().Substring(url.Scheme.Length));
                    Context.Response.Redirect(url.ToString());
                }
            }
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