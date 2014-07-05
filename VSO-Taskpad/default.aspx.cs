using SimplementE.VisualStudio.TaskPad.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VSO_Taskpad
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                var cred = new VsoBasicCredentials();
                string url = VsoWebServiceHelper.FormatUrl("simplement-e", "https://{account}.visualstudio.com/defaultcollection/_apis/projects?api-version=1.0-preview");
                string s = VsoWebServiceHelper.Raw(cred, url);
                lt.Text = s;
            }
        }
    }
}