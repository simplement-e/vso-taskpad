using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
//using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System.Web.Security;

namespace SimplementE.VisualStudio.TaskPad.Business
{
    public partial class ExternalAuthService : IHttpModule
    {

        internal static string CreateNewCSRFToken()
        {
            string o = HttpContext.Current.Session["_csrf_token_"] as string;
            if (string.IsNullOrEmpty(o))
            {
                MD5 md5 = MD5.Create() as MD5;
                string s = Convert.ToBase64String(md5.ComputeHash(Encoding.ASCII.GetBytes(new Random().Next().ToString())));
                HttpContext.Current.Session["_csrf_token_"] = s;
                return s;
            }
            return o;
        }

        internal static bool CheckCSRFToken(string token)
        {
            string o = HttpContext.Current.Session["_csrf_token_"] as string;
            if (string.IsNullOrEmpty(o)) return false;
            return o.Equals(token, StringComparison.InvariantCultureIgnoreCase);
        }

        internal static string GetRootUrl(HttpContext context)
        {
            string urlPrincipale = context.Request.Url.ToString();
            if (urlPrincipale.Contains("?"))
                urlPrincipale = urlPrincipale.Substring(0, urlPrincipale.IndexOf("?"));
            if(urlPrincipale.IndexOf("/oauth/")>=0)
                urlPrincipale = urlPrincipale.Substring(0, urlPrincipale.IndexOf("/oauth/"));
            if (!urlPrincipale.EndsWith("/"))
                urlPrincipale += "/";
            if (urlPrincipale.EndsWith("//"))
                urlPrincipale = urlPrincipale.Substring(0, urlPrincipale.Length - 1);

            return urlPrincipale.ToLower();
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpContext ctx = HttpContext.Current;

            string url = ctx.Request.Url.AbsolutePath;
            if (string.IsNullOrEmpty(url))
                return;
            url = VirtualPathUtility.ToAppRelative(url);
            url = url.ToLower();
            if (url.StartsWith("~/oauth/vstudio/start"))
            {
                ctx.RemapHandler(new VisualStudioOAuthStart());
            }
            else if (url.StartsWith("~/oauth/vstudio/process"))
            {
                ctx.RemapHandler(new VisualStudioOAuthProcess());
            }
        }

        void context_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpContext c = HttpContext.Current;
            if (c.User.Identity.IsAuthenticated)
                return;
        }

        //public static bool Login(UtilisateursDS ds)
        //{
        //    if (ds.security_utilisateurs.Count < 1)
        //        return false;

        //    FormsAuthentication.SetAuthCookie(ds.security_utilisateurs[0].uin_login, false);

        //    if (ds.security_utilisateurs.Count == 1)
        //    {
        //        GestionCommercialeServer.SetCurrentRjsId(ds.security_utilisateurs[0].uin_rjs_id,
        //            ds.security_utilisateurs[0].uin_uxid);
        //    }
        //    return true;
        //}

        private static string[] GetAdditionalScopes(HttpContext context)
        {
            string tmp = context.Request["scopes"];
            if (string.IsNullOrEmpty(tmp))
                return new string[0];
            
            
            string[] additionalScopes = tmp.Split(new char[] { ' ', ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries);
            return additionalScopes;
        }

        public void Dispose()
        {
        }
    }
}
