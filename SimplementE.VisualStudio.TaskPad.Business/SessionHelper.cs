using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SimplementE.VisualStudio.TaskPad.Business
{
    public static class UserSession
    {
        public static VsoWebServiceCredentials Credentials
        {
            get { return (HttpContext.Current != null && HttpContext.Current.Session != null) ? HttpContext.Current.Session["auth"] as VsoWebServiceCredentials : null; }
            set { if(HttpContext.Current!=null && HttpContext.Current.Session != null) HttpContext.Current.Session["auth"] = value; }
        }

        public static UserVsoAccount[] Accounts
        {
            get { return (HttpContext.Current != null && HttpContext.Current.Session != null) ? HttpContext.Current.Session["accounts"] as UserVsoAccount[] : null; }
            set { if (HttpContext.Current != null && HttpContext.Current.Session != null) HttpContext.Current.Session["accounts"] = value; }
        }

        public static VsoProject[] AllProjects
        {
            get { return (HttpContext.Current != null && HttpContext.Current.Session != null) ? HttpContext.Current.Session["projects"] as VsoProject[] : null; }
            set { if (HttpContext.Current != null && HttpContext.Current.Session != null) HttpContext.Current.Session["projects"] = value; }
        }

    }
}
