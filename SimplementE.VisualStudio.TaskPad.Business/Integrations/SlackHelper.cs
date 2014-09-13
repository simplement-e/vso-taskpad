using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimplementE.TaskPad.Business.Integrations
{
    public static class SlackHelper
    {
        [DataContract]
        private class playload
        {
            [DataMember]
            public string text { get; set; }

            [DataMember]
            public string username { get; set; }

            [DataMember]
            public string icon_url { get; set; }
            [DataMember]
            public string channel { get; set; }
        }

        public static void SendSimpleWebHookMessage(string text, string channel = null, string username=null, string userimg=null)
        {
            string url = MyConnectionStrings.Slack;
            if (string.IsNullOrEmpty(url))
                return;

            playload p = new playload();
            p.text = text;
            if (string.IsNullOrEmpty(username))
                p.username = "simplement-e";
            else
                p.username = username;
            if (string.IsNullOrEmpty(userimg))
                p.icon_url = "https://simplementedata.blob.core.windows.net/simplementesite/logo_avec_fond_bleu_144.png";
            else
                p.icon_url = userimg;
            if (!string.IsNullOrEmpty(channel))
                p.channel = channel;
            RestWebServiceClientHelper.Raw(url, "POST", "payload=" + JsonConvert.SerializeObject(p));
        }
    }
}
