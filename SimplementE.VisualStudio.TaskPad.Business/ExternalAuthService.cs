using SimplementE.VisualStudio.TaskPad.Business.VsoApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
//using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System.Web.Security;
using System.Web.SessionState;

namespace SimplementE.VisualStudio.TaskPad.Business
{
    public partial class AuthService : IHttpModule
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
#if DEBUG
            // seul vso-taskpad peut fonctionner sur notre version de démo
            return "https://vso-taskpad.azurewebsites.net/";
#else
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
#endif
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
            context.PostMapRequestHandler += context_PostMapRequestHandler;
            context.PostAcquireRequestState += context_PostAcquireRequestState;
        }
        void context_PostMapRequestHandler(object source, EventArgs e)
        {
            HttpApplication app = (HttpApplication)source;

            if (app.Context.Handler is IReadOnlySessionState || app.Context.Handler is IRequiresSessionState)
            {
                // no need to replace the current handler
                return;
            }

            // swap the current handler
            app.Context.Handler = new MyHttpHandler(app.Context.Handler);
        }

        private class MyHttpHandler : IHttpHandler, IRequiresSessionState
        {
            internal readonly IHttpHandler OriginalHandler;

            public MyHttpHandler(IHttpHandler originalHandler)
            {
                OriginalHandler = originalHandler;
            }

            public void ProcessRequest(HttpContext context)
            {
                // do not worry, ProcessRequest() will not be called, but let's be safe
                throw new InvalidOperationException("MyHttpHandler cannot process requests.");
            }

            public bool IsReusable
            {
                // IsReusable must be set to false since class has a member!
                get { return false; }
            }
        }

        void context_PostAcquireRequestState(object source, EventArgs e)
        {
            HttpApplication app = (HttpApplication)source;

            MyHttpHandler resourceHttpHandler = HttpContext.Current.Handler as MyHttpHandler;

            if (resourceHttpHandler != null)
            {
                // set the original handler back
                HttpContext.Current.Handler = resourceHttpHandler.OriginalHandler;
            }

            var ctx = ((HttpApplication)source);
            if (ctx.User.Identity.IsAuthenticated)
            {
                if (UserSession.Credentials == null)
                {
                    ReloadVsoCredentials(ctx);
                }

                var prjs = UserSession.AllProjects;
                if(prjs==null || prjs.Length==0)
                {
                    var usr = AccountsBll.GetUser(ctx.User.Identity.Name);
                    var accs = AccountsBll.GetAccounts(usr.Guid);
                    if(accs==null || accs.Count == 0)
                        AccountsBll.RefreshCurrentUserFromVso(usr.Email);
                    else
                    {
                        UserSession.Accounts = accs.ToArray();
                        UserSession.AllProjects = AccountsBll.GetAllProjects(usr.Guid).ToArray();
                    }
                }
            }
        }

        private void ReloadVsoCredentials(HttpApplication ctx)
        {
            var usr = AccountsBll.GetUser(ctx.User.Identity.Name);
            if (usr == null)
                return;

            if (!string.IsNullOrEmpty(usr.VsoAccessToken))
            {
                try
                {
                    var auth = new VsoOauthCredentials(usr.VsoAccessToken);
                    var p = Profiles.GetMe(auth);
                    UserSession.Credentials = auth;
                    return;
                }
                catch (Exception)
                {

                }
            }

            if (!string.IsNullOrEmpty(usr.VsoRefreshToken))
            {
                try
                {
                    var token = AuthService.VisualStudioOAuthProcess.RefreshToken(usr.VsoRefreshToken);
                    AccountsBll.RefreshTokens(usr.Email, token.access_token, token.refresh_token, token.expires_in);
                    UserSession.Credentials = new VsoOauthCredentials(token.access_token);
                }
                catch (ApplicationException)
                {
                    AccountsBll.RefreshTokens(usr.Email, null, null, 0);
                }
            }

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
