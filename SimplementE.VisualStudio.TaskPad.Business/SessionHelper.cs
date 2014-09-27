using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SimplementE.TaskPad.Business
{
    public static class UserSession
    {
        public static VsoWebServiceCredentials Credentials
        {
            get { return (HttpContext.Current != null && HttpContext.Current.Session != null) ? HttpContext.Current.Session["auth"] as VsoWebServiceCredentials : null; }
            set { if(HttpContext.Current!=null && HttpContext.Current.Session != null) HttpContext.Current.Session["auth"] = value; }
        }

        public static UserAccount[] Accounts
        {
            get { return (HttpContext.Current != null && HttpContext.Current.Session != null) ? HttpContext.Current.Session["accounts"] as UserAccount[] : null; }
            set { if (HttpContext.Current != null && HttpContext.Current.Session != null) HttpContext.Current.Session["accounts"] = value; }
        }

        public static Project[] AllProjects
        {
            get { return (HttpContext.Current != null && HttpContext.Current.Session != null) ? HttpContext.Current.Session["projects"] as Project[] : null; }
            set { if (HttpContext.Current != null && HttpContext.Current.Session != null) HttpContext.Current.Session["projects"] = value; }
        }


        public static UserAccount GetAccountFromProject(Project p)
        {
            return (from ac in Accounts
                    where ac.Guid.Equals(p.AccountGuid)
                    select ac).FirstOrDefault();
        }
    }
}
