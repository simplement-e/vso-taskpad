using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace VSO_Taskpad.app
{
    /// <summary>
    /// Description résumée de vsocallback
    /// </summary>
    public class vsocallback : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}