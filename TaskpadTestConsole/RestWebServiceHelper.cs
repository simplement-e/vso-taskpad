using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaskpadTestConsole
{
    public static class RestWebServiceClientHelper
    {
        /// <summary>
        /// Async call to a VSO webservice
        /// </summary>
        /// <param name="credentials">Credentials for the call</param>
        /// <param name="url">url to call</param>
        /// <param name="verb">http verb to use</param>
        /// <returns>The unparsed string returned by the service</returns>
        public async static Task<string> RawAsync(string url, string verb = "GET", string data = null)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(verb))
                verb = "GET";

            HttpWebResponse rsp = null;
            HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
            req.Method = verb;

            switch (verb.ToUpper())
            {
                case "GET":
                    if (data != null)
                        throw new ApplicationException("If you're using a GET request, data must be null");
                    break;
                case "POST":
                    if (data != null)
                    {
                        req.ContentType = "application/x-www-form-urlencoded";
                        StreamWriter stOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
                        stOut.Write(data);
                        stOut.Close();
                    }
                    break;
            }

            try
            {
                rsp = await req.GetResponseAsync() as HttpWebResponse;
                string st = null;
                using (StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream()))
                    st = stIn.ReadToEnd();
                return st;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string Raw(string url, string verb = "GET", string data = null)
        {
            try
            {
                var t = RawAsync(url, verb, data);
                t.Wait();
                if (t.Exception != null)
                    throw t.Exception;
                return t.Result;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                else
                    throw;
            }
        }
    }

}
