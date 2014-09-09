using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace VSO_Taskpad.App_Start
{
    public class RouteConfig
    {
        public static void Register()
        {
            RouteTable.Routes.MapPageRoute("Bugs", "app/{project}/bugs", "~/app/bugs.aspx");
        }
    }
}