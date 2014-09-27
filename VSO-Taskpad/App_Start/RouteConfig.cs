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
            RouteTable.Routes.MapPageRoute("AllRoadmap", "app/roadmap", "~/app/globalroadmap.aspx");

            RouteTable.Routes.MapPageRoute("Roadmap", "app/{project}/roadmap", "~/app/roadmap.aspx");
            RouteTable.Routes.MapPageRoute("Bugs", "app/{project}/bugs", "~/app/bugs.aspx");
            RouteTable.Routes.MapPageRoute("Backlog", "app/{project}/backlog", "~/app/backlog.aspx");
            RouteTable.Routes.MapPageRoute("ProjectHome", "app/{project}", "~/app/default.aspx");
        }
    }
}