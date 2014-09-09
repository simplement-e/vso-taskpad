using SimplementE.VisualStudio.TaskPad.Business;
using SimplementE.VisualStudio.TaskPad.Business.VsoApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace VSO_Taskpad.Account
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        [Route("refreshdata"), HttpGet]
        public bool RefreshData()
        {
            VsoWebServiceCredentials vso = HttpContext.Current.Session["auth"] as VsoWebServiceCredentials;
            if (vso == null)
                return false;

            var usr = AccountsBll.GetUser(User.Identity.Name);
            if (usr == null)
                return false;

            var accounts = Profiles.GetAccounts(vso);
            AccountsBll.RefreshAccounts(usr.Guid, accounts);

            return true;
        }
    }
}