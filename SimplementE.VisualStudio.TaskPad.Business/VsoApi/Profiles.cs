using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplementE.VisualStudio.TaskPad.Business.VsoApi
{
    public class Profiles
    {
        public static Profile GetMe(VsoWebServiceCredentials cred)
        {
            var r = VsoWebServiceHelper.Raw(cred, "https://app.vssps.visualstudio.com/_apis/profile/profiles/me?api-version=1.0-preview.1");
            return new Profile() { };
        }

        public static IEnumerable<Account> GetAccounts(VsoWebServiceCredentials cred)
        {
            var r = VsoWebServiceHelper.Raw(cred, "https://app.vssps.visualstudio.com/_apis/accounts?api-version=1.0-preview.1");

            return null;
        }

        [Serializable]
        private class ProfileResult
        {
            //public WorkItemData[] value { get; set; }
        }


        [Serializable]
        public class Profile
        {
            public string displayName { get; set; }
            public Guid id { get; set; }
        }

        [Serializable]
        public class Account
        {
            public string name { get; set; }
            public Guid id { get; set; }
        }

    }

}
