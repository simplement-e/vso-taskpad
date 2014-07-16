namespace SimplementE.VisualStudio.TaskPad.Business
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Core.EntityClient;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.IO;
    using System.Xml;
    using System.Configuration;

    public class TaskPadDbContext : DbContext
    {
        private static string _cnString = null;
        internal static string FromSettings()
        {
            if (_cnString != null)
                return _cnString;
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
                    XmlElement elmC = doc.SelectSingleNode("/Credentials/ConnectionString") as XmlElement;
                    if (elmC != null)
                    {
                        _cnString = elmC.InnerText;
                    }
                }
            }
            catch
            {

            }
#else
            try {
            _cnString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
            }
            catch {
            }
#endif

            if (_cnString == null)
                throw new ApplicationException("Invalid Config");
            return _cnString;
        }

        public static TaskPadDbContext Get()
        {
            return Get(FromSettings());
        }

        public static TaskPadDbContext Get(string sqlServerConnectionString)
        {
            System.Data.SqlClient.SqlConnectionStringBuilder scsb = new System.Data.SqlClient.SqlConnectionStringBuilder(sqlServerConnectionString);

            EntityConnectionStringBuilder ecb = new EntityConnectionStringBuilder();
            //ecb.Metadata = "res://*/TaskPadDBContext.csdl|res://*/TaskPadDBContext.ssdl|res://*/TaskPadDBContext.msl";
            ecb.Provider = "System.Data.SqlClient";
            ecb.ProviderConnectionString = scsb.ConnectionString;

            return new TaskPadDbContext(sqlServerConnectionString);
        }

        // Votre contexte a été configuré pour utiliser une chaîne de connexion « TaskPadDbContext » du fichier 
        // de configuration de votre application (App.config ou Web.config). Par défaut, cette chaîne de connexion cible 
        // la base de données « SimplementE.VisualStudio.TaskPad.Business.TaskPadDbContext » sur votre instance LocalDb. 
        // 
        // Pour cibler une autre base de données et/ou un autre fournisseur de base de données, modifiez 
        // la chaîne de connexion « TaskPadDbContext » dans le fichier de configuration de l'application.
        private TaskPadDbContext(string connectionString)
            : base(connectionString)
        {
        }

        // Ajoutez un DbSet pour chaque type d'entité à inclure dans votre modèle. Pour plus d'informations 
        // sur la configuration et l'utilisation du modèle Code First, consultez http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Tenants> Tenants { get; set; }
    }

    public class Tenants
    {
        [Key]
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Tenant { get; set; }
    }
}