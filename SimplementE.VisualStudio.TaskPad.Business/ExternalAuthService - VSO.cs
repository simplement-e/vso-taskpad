using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Security.Cryptography;
using System.Net;
using System.Runtime.Serialization;
using System.IO;
using Newtonsoft.Json;

namespace SimplementE.VisualStudio.TaskPad.Business
{
    partial class AuthService
    {
        public class VisualStudioOAuthStart : IHttpHandler, IRequiresSessionState
        {
            public bool IsReusable
            {
                get { return false; }
            }

            public void ProcessRequest(HttpContext context)
            {
                string urlPrincipale = GetRootUrl(context);

                if (!context.User.Identity.IsAuthenticated)
                {
                    context.Response.Redirect("~/", false);
                    return;
                }

                string dt = CreateNewCSRFToken();
                StringBuilder blr = new StringBuilder();
                blr.Append("https://app.vssps.visualstudio.com/oauth2/authorize?client_id=");
                blr.Append(MyConnectionStrings.VisualStudioAppId);
                blr.Append("&response_type=Assertion&scope=preview_api_all%20preview_msdn_licensing");
                blr.Append("&redirect_uri=");
                StringBuilder blrUri = new StringBuilder();
                blrUri.Append(urlPrincipale);
                blrUri.Append("oauth/vstudio/process/");
                blr.Append(HttpUtility.UrlEncode(blrUri.ToString()));
                if (!string.IsNullOrEmpty(context.Request.Url.Query))
                {
                    blr.Append("&state=");
                    blr.Append(HttpUtility.UrlEncode(context.Request.Url.Query));
                }
                context.Response.Redirect(blr.ToString());
            }
        }

        public class VisualStudioOAuthProcess : IHttpHandler, IRequiresSessionState
        {
            public void ProcessRequest(HttpContext context)
            {
                string urlPrincipale = GetRootUrl(context);
                context.Response.ContentType = "text/plain";

                if (!context.User.Identity.IsAuthenticated)
                {
                    context.Response.Redirect("~/", false);
                    return;
                }

                string error = context.Request["error"];
                if (!string.IsNullOrWhiteSpace(error))
                {
                    context.Response.Redirect("~/oauthError.aspx?kind=" + HttpUtility.UrlEncode(error));
                    return;
                }


                string code = context.Request["code"];

                string urlRet = context.Request.RawUrl;

                StringBuilder blrUri = new StringBuilder();
                blrUri.Append(urlPrincipale);
                blrUri.Append("oauth/vstudio/process/");

                TokenResponse token = GetTokenFromCode(code, urlPrincipale, blrUri.ToString(), context); ;

                if (string.IsNullOrEmpty(token.access_token))
                    context.Response.Redirect("~/oauthError.aspx?kind=access_denied");

                AccountsBll.RefreshTokens(context.User.Identity.Name, token.access_token, token.refresh_token, token.expires_in);

                context.Session["auth"] = new VsoOauthCredentials(token.access_token);


                context.Response.Redirect("~/");


            }

            [DataContract]
            public class TokenResponse
            {
                [DataMember]
                public string token_type { get; set; }
                [DataMember]
                public int expires_in { get; set; }
                [DataMember]
                public string refresh_token { get; set; }
                [DataMember]
                public string access_token { get; set; }
            }

            private TokenResponse GetTokenFromCode(string code, string urlPrincipale, string calledUrl, HttpContext context)
            {
                try
                {
                    HttpWebRequest rq = HttpWebRequest.Create("https://app.vssps.visualstudio.com/oauth2/token") as HttpWebRequest;
                    rq.Method = "POST";
                    rq.ContentType = "application/x-www-form-urlencoded";

                    StringBuilder blr = new StringBuilder();
                    blr.Append("client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
                    blr.Append("&client_assertion=");
                    blr.Append(MyConnectionStrings.VisualStudioAppSecret);
                    blr.Append("&grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer");
                    blr.Append("&assertion=");
                    blr.Append(code);

                    StringBuilder blrUri = new StringBuilder();
                    blrUri.Append(urlPrincipale);
                    blrUri.Append("oauth/vstudio/process/");

                    blr.Append("&redirect_uri=");
                    blr.Append(blrUri.ToString());

                    string st = blr.ToString();
                    rq.ContentLength = st.Length;

                    StreamWriter stOut = new StreamWriter(rq.GetRequestStream(), System.Text.Encoding.ASCII);
                    stOut.Write(st);
                    stOut.Close();

                    StreamReader stIn = new StreamReader(rq.GetResponse().GetResponseStream());
                    st = stIn.ReadToEnd();
                    stIn.Close();

                    TokenResponse tk = JsonConvert.DeserializeObject<TokenResponse>(st);
                    return tk;
                }
                catch (Exception ex)
                {
                }

                return null;
            }


            [DataContract]
            private class LiveUser
            {
                [DataMember]
                public string id { get; set; }
                [DataMember]
                public string name { get; set; }
                [DataMember]
                public string first_name { get; set; }
                [DataMember]
                public string last_name { get; set; }
                [DataMember]
                public string email { get; set; }
                [DataMember]
                public string link { get; set; }
            }




            public bool IsReusable
            {
                get
                {
                    return false;
                }
            }

            public static TokenResponse RefreshToken(string refreshToken)
            {
                string urlPrincipale = GetRootUrl(HttpContext.Current);
                HttpWebRequest rq = HttpWebRequest.Create("https://app.vssps.visualstudio.com/oauth2/token") as HttpWebRequest;
                rq.Method = "POST";
                rq.ContentType = "application/x-www-form-urlencoded";

                StringBuilder blr = new StringBuilder();
                blr.Append("client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
                blr.Append("&client_assertion=");
                blr.Append(MyConnectionStrings.VisualStudioAppSecret);
                blr.Append("&grant_type=refresh_token");
                blr.Append("&assertion=");
                blr.Append(refreshToken);

                StringBuilder blrUri = new StringBuilder();
                blrUri.Append(urlPrincipale);
                blrUri.Append("oauth/vstudio/process/");

                blr.Append("&redirect_uri=");
                blr.Append(blrUri.ToString());

                string st = blr.ToString();
                rq.ContentLength = st.Length;

                StreamWriter stOut = new StreamWriter(rq.GetRequestStream(), System.Text.Encoding.ASCII);
                stOut.Write(st);
                stOut.Close();

                try
                {
                    WebResponse resp = rq.GetResponse() as WebResponse;
                    using (StreamReader stIn = new StreamReader(resp.GetResponseStream()))
                    {
                        st = stIn.ReadToEnd();
                        stIn.Close();
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Response.ContentLength > 0)
                    {
                        using (StreamReader stIn = new StreamReader(ex.Response.GetResponseStream()))
                        {
                            string err = stIn.ReadToEnd();
                            throw new ApplicationException(err);
                        }

                    }
                }
                TokenResponse tk = JsonConvert.DeserializeObject<TokenResponse>(st);


                return tk;
            }
        }
    }
}
