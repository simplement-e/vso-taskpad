using SimplementE.TaskPad.Business;
using SimplementE.TaskPad.Business.VsoApi;
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
            if (!User.Identity.IsAuthenticated)
                return false;
            return AccountsBll.RefreshCurrentUserFromVso(User.Identity.Name);
        }
    }
}