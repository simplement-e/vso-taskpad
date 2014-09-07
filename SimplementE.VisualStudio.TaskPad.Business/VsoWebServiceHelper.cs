using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
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
        private string _account;

        public string Account { get; internal set; }
        /// <summary>
        /// Overrides this to provides credentials to the underlying web service request
        /// </summary>
        /// <param name="client">The Web service request to authenticate</param>
        protected internal abstract void AddAuth(HttpWebRequest client);
    }

    /// <summary>
    /// Basic (alternate) credentials for VSO
    /// </summary>
    [Serializable]
    public sealed class VsoBasicCredentials : VsoWebServiceCredentials, ISerializable
    {
        private string _username;
        private string _password;

        /// <summary>
        /// Creates an instance of the <see cref="VsoBasicCredentials"/> class using appsettings
        /// </summary>
        /// <remarks><para>This constructor is for use on dev stations or on-premise deployments</para> 
        /// <para>The following settings are used :
        /// <list type="table">
        /// <item><term>vso-username</term><description>the username</description></item>
        /// <item><term>vso-password</term><description>the password</description></item>
        /// </list></para>
        /// </remarks>
        public VsoBasicCredentials()
        {
            var cfg = ConfigurationManager.AppSettings;
            _username = cfg.Get("vso-username");
            _password = cfg.Get("vso-password");
            Account = cfg.Get("vso-account");
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
                    elmU = doc.SelectSingleNode("/Credentials/Account") as XmlElement;
                    if (elmU != null)
                    {
                        Account = elmU.InnerText;
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

        internal VsoBasicCredentials(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            _username = info.GetString("Username");
            _password = info.GetString("Password");
        }


        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            info.AddValue("Username", _username);
            info.AddValue("Password", _password);
        }
    }

    /// <summary>
    /// Basic (alternate) credentials for VSO
    /// </summary>
    [Serializable]
    public sealed class VsoOauthCredentials : VsoWebServiceCredentials, ISerializable
    {
        private string _bearer = null;
        /// <summary>
        /// Creates an instance of the <see cref="VsoBasicCredentials"/> class using appsettings
        /// </summary>
        /// <remarks><para>This constructor is for use on dev stations or on-premise deployments</para> 
        /// <para>The following settings are used :
        /// <list type="table">
        /// <item><term>vso-username</term><description>the username</description></item>
        /// <item><term>vso-password</term><description>the password</description></item>
        /// </list></para>
        /// </remarks>
        public VsoOauthCredentials(string bearerToken)
        {
            var cfg = ConfigurationManager.AppSettings;
            _bearer = bearerToken;
            Account = "simplement-e";
        }

      

        /// <summary>
        /// Overrides this to provides credentials to the underlying web service request
        /// </summary>
        /// <param name="client">The Web service request to authenticate</param>
        protected internal override void AddAuth(HttpWebRequest client)
        {
            client.Headers.Add("Authorization", "Bearer " + _bearer);
        }

        internal VsoOauthCredentials(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            _bearer = info.GetString("bearer");
        }


        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            info.AddValue("bearer", _bearer);
        }
    }


    [Serializable]
    public class VsoJsonResult<T>
    {
        public T value { get; set; }
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

            url = FormatUrl(credentials.Account, url);

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

        public static VsoJsonResult<T> Call<T>(VsoWebServiceCredentials credentials, string url, string verb = "GET")
        {
            string s = Raw(credentials, url, verb);
            return JsonConvert.DeserializeObject<VsoJsonResult<T>>(s);
        }


        /// <summary>
        /// Sync call to a VSO webservice
        /// </summary>
        /// <param name="credentials">Credentials for the call</param>
        /// <param name="url">url to call</param>
        /// <param name="verb">http verb to use</param>
        /// <returns>The unparsed string returned by the service</returns>
        public static string Raw(VsoWebServiceCredentials credentials, string url, string verb = "GET", string body = null)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (credentials == null)
                throw new ArgumentNullException("credentials");
            if (string.IsNullOrEmpty(verb))
                verb = "GET";
            if (!string.IsNullOrEmpty(body) && verb.Equals("GET"))
                throw new InvalidOperationException("Can't GET if you have a request body to send.");


            url = FormatUrl(credentials.Account, url);

            HttpWebResponse rsp = null;
            HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
            //req.TransferEncoding = "UTF8";
            req.Method = verb;
            req.ContentType = "application/json";
            if (!string.IsNullOrEmpty(body))
            {
                using (var s = req.GetRequestStream())
                {
                    var bs = Encoding.UTF8.GetBytes(body);
                    s.Write(bs, 0, bs.Length);
                }
            }
            credentials.AddAuth(req);
            try
            {
                rsp = req.GetResponse() as HttpWebResponse;
                return ParseResponse(rsp);
            }
            catch (WebException e)
            {
                if(e.Response.ContentLength>0)
                {
                    if (!e.Response.ContentType.StartsWith("text") && !e.Response.ContentType.StartsWith("application/json"))
                        throw;
                    using(var r = new StreamReader(e.Response.GetResponseStream()))
                    {
                        string msg = r.ReadToEnd();
                        throw new ApplicationException(msg);
                    }
                }
                throw;
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
