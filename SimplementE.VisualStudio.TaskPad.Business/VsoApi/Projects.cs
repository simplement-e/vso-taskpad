using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplementE.VisualStudio.TaskPad.Business.VsoApi
{
    public static class Projects
    {
        public static Project[] GetProjects(VsoWebServiceCredentials cred)
        {
            var r = VsoWebServiceHelper.Call<Project[]>(cred, "https://{account}.visualstudio.com/defaultcollection/_apis/projects?api-version=1.0-preview");
            return r.value;
        }
    }

    [Serializable]
    public class Project
    {
        public string name { get; set; }
        public Guid id { get; set; }
        public string url { get; set; }
    }
}
