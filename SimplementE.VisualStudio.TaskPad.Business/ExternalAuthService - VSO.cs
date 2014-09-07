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
    partial class ExternalAuthService 
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

                string moreScopes = null;

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

                string error = context.Request["error"];
                if (!string.IsNullOrWhiteSpace(error))
                {
                    context.Response.Redirect("~/oauthError.aspx?error=" + HttpUtility.UrlEncode(error));
                    return;
                }


                string code = context.Request["code"];

                string urlRet = context.Request.RawUrl;

                StringBuilder blrUri = new StringBuilder();
                blrUri.Append(urlPrincipale);
                blrUri.Append("oauth/msaccount/process/");

                TokenResponse token = GetTokenFromCode(code, urlPrincipale, blrUri.ToString(), context); ;

                if (string.IsNullOrEmpty(token.access_token))
                    context.Response.Redirect("~/oauthError.aspx?error=Token%20invalide");

                // do something with the token...
            }

            [DataContract]
            private class TokenResponse
            {
                [DataMember]
                public string token_type { get; set; }
                [DataMember]
                public string expires_in { get; set; }
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
                    context.Response.Write(JsonConvert.SerializeObject(tk));
                    return tk;
                }
                catch(Exception ex)
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
        }
    }
}
