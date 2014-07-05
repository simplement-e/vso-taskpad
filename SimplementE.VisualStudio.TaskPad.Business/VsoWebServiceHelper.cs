using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace SimplementE.VisualStudio.TaskPad.Business
{
    /// <summary>
    /// Base class for VSO authentication
    /// </summary>
    public abstract class VsoWebServiceCredentials
    {
        /// <summary>
        /// Overrides this to provides credentials to the underlying web service request
        /// </summary>
        /// <param name="client">The Web service request to authenticate</param>
        protected internal abstract void AddAuth(HttpWebRequest client);
    }

    /// <summary>
    /// Basic (alternate) credentials for VSO
    /// </summary>
    public sealed class VsoBasicCredentials : VsoWebServiceCredentials
    {
        private string _username;
        private string _password;

        /// <summary>
        /// Creates an instance of the <see cref="VsoBasicCredentials"/> class using appsettings
        /// </summary>
        /// <remarks>The following settings are used :
        /// <list type="table">
        /// <item><term>vso-username</term><description>the username</description></item>
        /// <item><term>vso-password</term><description>the password</description></item>
        /// </list></remarks>
        public VsoBasicCredentials()
        {
            var cfg = ConfigurationManager.AppSettings;
            _username = cfg.Get("vso-username");
            _password = cfg.Get("vso-password");
#if DEBUG
            try
            {
                string pth = "TestCreds.xml";
                if (HttpContext.Current != null)
                {
                    pth = HttpContext.Current.Request.MapPath("~/TestCreds.xml");
                }
                
                if (File.Exists(pth))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(pth);
                    XmlElement elmU = doc.SelectSingleNode("/Credentials/Username") as XmlElement;
                    XmlElement elmP = doc.SelectSingleNode("/Credentials/Password") as XmlElement;
                    if (elmU != null && elmP != null)
                    {
                        _username = elmU.InnerText;
                        _password = elmP.InnerText;
                    }

                }
            }
            catch
            {

            }
#endif
        }

        /// <summary>
        /// Creates an instance of the <see cref="VsoBasicCredentials"/> class
        /// </summary>
        /// <param name="username">the username</param>
        /// <param name="password">the password</param>
        public VsoBasicCredentials(string username, string password)
        {
            _username = username;
            _password = password;
        }

        /// <summary>
        /// Overrides this to provides credentials to the underlying web service request
        /// </summary>
        /// <param name="client">The Web service request to authenticate</param>
        protected internal override void AddAuth(HttpWebRequest client)
        {
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(_username + ":" + _password));
            client.Headers.Add("Authorization", "Basic " + encoded);
        }
    }

    /// <summary>
    /// Helper class for calling VSO web services
    /// </summary>
    public static class VsoWebServiceHelper
    {
        /// <summary>
        /// update an url from https://{account}.visualstudio.com/... 
        /// to the real url by replacing {account} or {0} by provided account name
        /// </summary>
        /// <param name="accountName">account</param>
        /// <param name="url">url to update</param>
        /// <returns>The url with a simple remplacement from {account} to the real
        /// account name</returns>
        public static string FormatUrl(string accountName, string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            url = url.ToLower();
            if (url.Contains("{account}"))
                url = url.Replace("{account}", "{0}");

            return string.Format(url, accountName);
        }

        /// <summary>
        /// Async call to a VSO webservice
        /// </summary>
        /// <param name="credentials">Credentials for the call</param>
        /// <param name="url">url to call</param>
        /// <param name="verb">http verb to use</param>
        /// <returns>The unparsed string returned by the service</returns>
        public async static Task<string> RawAsync(VsoWebServiceCredentials credentials, string url, string verb = "GET")
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (credentials == null)
                throw new ArgumentNullException("credentials");
            if (string.IsNullOrEmpty(verb))
                verb = "GET";

            HttpWebResponse rsp = null;
            HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
            req.Method = verb;
            credentials.AddAuth(req);
            try
            {
                rsp = await req.GetResponseAsync() as HttpWebResponse;
                return ParseResponse(rsp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Sync call to a VSO webservice
        /// </summary>
        /// <param name="credentials">Credentials for the call</param>
        /// <param name="url">url to call</param>
        /// <param name="verb">http verb to use</param>
        /// <returns>The unparsed string returned by the service</returns>
        public static string Raw(VsoWebServiceCredentials credentials, string url, string verb = "GET")
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (credentials == null)
                throw new ArgumentNullException("credentials");
            if (string.IsNullOrEmpty(verb))
                verb = "GET";

            HttpWebResponse rsp = null;
            HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
            req.Method = verb;
            credentials.AddAuth(req);
            try
            {
                rsp = req.GetResponse() as HttpWebResponse;
                return ParseResponse(rsp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string ParseResponse(HttpWebResponse rsp)
        {
            using (StreamReader rdr = new StreamReader(rsp.GetResponseStream()))
            {
                return rdr.ReadToEnd();
            }
        }
    }
}
