using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace SimplementE.VisualStudio.TaskPad.Business
{
    static class MyConnectionStrings
    {
        static MyConnectionStrings()
        {
            string pth;
            if (HttpContext.Current != null)
            {
                pth = HttpContext.Current.Server.MapPath("/Connections.xml");
            }
            else
            {
                pth = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Connections.xml");
            }
            if (File.Exists(pth))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(pth);

                XmlElement elm = doc.SelectSingleNode("/ConnectionStrings/Database") as XmlElement;
                if (elm != null)
                    Database = elm.InnerText;

                elm = doc.SelectSingleNode("/ConnectionStrings/VisualStudio") as XmlElement;
                if (elm != null)
                {
                    XmlElement elm2 = elm.SelectSingleNode("Key") as XmlElement;
                    if(elm2!=null)
                        VisualStudioAppId = elm2.InnerText.Trim();
                    elm2 = elm.SelectSingleNode("Secret") as XmlElement;
                    if (elm2 != null)
                        VisualStudioAppSecret = elm2.InnerText.Trim();
                }
            }
        }

        public static string Database { get; private set; }
        public static string VisualStudioAppId { get; private set; }
        public static string VisualStudioAppSecret { get; private set; }
    }
}
