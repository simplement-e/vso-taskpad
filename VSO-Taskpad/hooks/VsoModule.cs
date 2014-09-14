using Newtonsoft.Json;
using SimplementE.TaskPad.Business;
using SimplementE.TaskPad.Business.Integrations;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

namespace VSO_Taskpad.hooks
{
    public class VsoModule : IHttpModule
    {
        /// <summary>
        /// Vous devrez configurer ce module dans le fichier Web.config de votre
        /// projet Web et l'inscrire auprès des services IIS (Internet Information Services) pour pouvoir l'utiliser. Pour plus d'informations
        /// consultez le lien suivant : http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //code de nettoyage ici.
        }

        public void Init(HttpApplication context)
        {
            // Exemple ci-dessous montrant comment gérer l'événement LogRequest et fournir 
            // une implémentation de journalisation personnalisée pour celui-ci
            context.BeginRequest += context_BeginRequest;
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpContext ctx = HttpContext.Current;

            string url = ctx.Request.Url.AbsolutePath;
            if (string.IsNullOrEmpty(url))
                return;
            url = VirtualPathUtility.ToAppRelative(url);
            url = url.ToLower();

            switch (url)
            {
                case "~/hooks/vso/checkedin":
                    ctx.RemapHandler(new VsoCheckedInHandler());
                    break;
                case "~/hooks/vso/buildcompleted":
                    ctx.RemapHandler(new VsoBuildCompletedHandler());
                    break;
            }
        }

        #endregion
        private class MessagePayload
        {
            public string text { get; set; }
        }


        private class VsoCheckedInHandler : IHttpHandler
        {
            public bool IsReusable
            {
                get { return false; }
            }

            private class CheckedInPayload
            {
                public string subscriptionId { get; set; }
                public string eventType { get; set; }
                public MessagePayload message { get; set; }
                public ResourcePayload resource { get; set; }

                public override string ToString()
                {
                    return "new event '" + eventType + "' : " + ((message==null)?"-":message.text);
                }
            }

            private class ResourcePayload
            {
                public int changesetId { get; set; }
                public string url { get; set; }
                public string comment { get; set; }
            }

            public void ProcessRequest(HttpContext context)
            {
                CheckedInPayload p = null;
                using (StreamReader rdr = new StreamReader(context.Request.InputStream))
                {
                    string tmp = rdr.ReadToEnd();
                    p = JsonConvert.DeserializeObject<CheckedInPayload>(tmp);
                }

                string project = context.Request["project"];
                Logger.LogInfo(p.ToString());

                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        StringBuilder blr = new StringBuilder();
                        //blr.Append("<");
                        //blr.Append(p.resource.url);
                        //blr.Append("|");
                        blr.Append("Check-in #");
                        blr.Append(p.resource.changesetId);
                        //blr.Append(">");

                        blr.Append(" sur le projet ");
                        blr.Append(project);
                        blr.Append(" : ");
                        blr.Append(p.resource.comment);

                        SlackHelper.SendSimpleWebHookMessage(blr.ToString());
                    }
                    catch
                    {

                    }
                });
            }
        }

        private class VsoBuildCompletedHandler : IHttpHandler
        {
            public bool IsReusable
            {
                get { return false; }
            }

            private class CheckedInPayload
            {
                public string subscriptionId { get; set; }
                public string eventType { get; set; }
                public MessagePayload message { get; set; }
                public ResourcePayload resource { get; set; }

                public override string ToString()
                {
                    return "new event '" + eventType + "' : " + ((message == null) ? "-" : message.text);
                }
            }

            private class BuildDefinitionPayload
            {
                public string name { get; set; }
            }

            private class ResourcePayload
            {
                public DateTime startTime { get; set; }
                public DateTime finishTime { get; set; }
                public string status { get; set; }
                public string buildNumber { get; set; }
                public string url { get; set; }
                public string comment { get; set; }
                public BuildDefinitionPayload definition { get; set; }
            }

            public void ProcessRequest(HttpContext context)
            {
                CheckedInPayload p = null;
                using (StreamReader rdr = new StreamReader(context.Request.InputStream))
                {
                    string tmp = rdr.ReadToEnd();
                    p = JsonConvert.DeserializeObject<CheckedInPayload>(tmp);
                }

                string project = context.Request["project"];
                Logger.LogInfo(p.ToString());

                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        StringBuilder blr = new StringBuilder();
                        //blr.Append("<");
                        //blr.Append(p.resource.url);
                        //blr.Append("|");
                        blr.Append("Build ");
                        blr.Append(p.resource.definition.name);
                        //blr.Append(p.resource.changesetId);
                        //blr.Append(">");
                        blr.Append(" sur le projet ");
                        blr.Append(project);
                        blr.Append(" termine avec le statut *");
                        blr.Append(p.resource.status);
                        blr.Append("*");
                        blr.Append(p.resource.comment);

                        SlackHelper.SendSimpleWebHookMessage(blr.ToString());
                    }
                    catch
                    {

                    }
                });
            }
        }


    }
}
