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

    internal class TaskPadDbContext : DbContext
    {
        private static string _cnString = null;
        internal static string FromSettings()
        {
            _cnString = MyConnectionStrings.Database;
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

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserVsoAccount> UserVsoAccounts { get; set; }
    }

    public class User
    {
        [Key]
        public Guid Guid { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }

        public string VsoAccessToken { get; set; }
        public string VsoRefreshToken { get; set; }
        public Guid LastUserVsoAccountGuid { get; set; }

        //public virtual UserVsoAccount LastUserVsoAccount { get; set; }
    }

    public class UserVsoAccount
    {
        [Key]
        public Guid Guid { get; set; }
        [Required]
        public Guid UserGuid { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Label { get; set; }

        //public virtual User User { get; set; }

    }
}