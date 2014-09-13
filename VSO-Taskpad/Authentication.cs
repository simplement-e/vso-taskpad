using SimplementE.TaskPad.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace VSO_Taskpad
{
    [RoutePrefix("api/auth"), AllowAnonymous]
    public class AuthenticationController : ApiController
    {
        [Route("login"), HttpGet]
        public bool Login(string login, string pwd)
        {
            var usr = AccountsBll.TryLogin(login, pwd);
            if (usr!=null)
            {
                FormsAuthentication.SetAuthCookie(usr.Email, false);
                return true;
            }
            return false;
        }

        [Route("create"), HttpGet]
        public bool Login(string name, string login, string pwd)
        {
            Guid g = AccountsBll.RegisterNewUser(name, login, pwd);
            if (!Guid.Empty.Equals(g))
            {
                FormsAuthentication.SetAuthCookie(login, false);
                return true;
            }
            return false;
        }
    }
}