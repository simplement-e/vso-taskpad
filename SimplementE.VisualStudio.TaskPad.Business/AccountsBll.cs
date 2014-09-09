using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimplementE.VisualStudio.TaskPad.Business
{
    public static class AccountsBll
    {
        public static User TryLogin(string username, string pwd)
        {
            username = username.ToLower();
            string encpwd = ComputePwd(username, pwd);

            using (var db = TaskPadDbContext.Get())
            {
                var usr = (from z in db.Users
                           where z.Email.Equals(username) && z.Password.Equals(encpwd)
                           select z).FirstOrDefault();
                if (usr != null)
                {
                    usr.Password = null;
                    return usr;
                }
            }

            return null;
        }

        public static void RefreshTokens(string username, string accessToken, string refreshToken, int ttl)
        {
            DateTimeOffset off = DateTimeOffset.Now.AddSeconds(ttl);
            username = username.ToLower();
            using (var db = TaskPadDbContext.Get())
            {
                var usr = (from z in db.Users
                           where z.Email.Equals(username)
                           select z).FirstOrDefault();
                if (usr != null)
                {
                    usr.VsoAccessToken = accessToken;
                    usr.VsoRefreshToken = refreshToken;
                    usr.VsoAccessTokenExpiration = off;
                    db.SaveChanges();
                }
            }
        }

        private static string ComputePwd(string username, string pwd)
        {
            username = username.ToLower();
            StringBuilder blrpwd = new StringBuilder();

            for (int i = 0; i < pwd.Length; i++)
            {
                blrpwd.Append(pwd[i]);
                blrpwd.Append(username[i % (username.Length)]);
            }

            SHA1Managed mg = new SHA1Managed();
            string encpwd = Convert.ToBase64String(mg.ComputeHash(Encoding.UTF8.GetBytes(blrpwd.ToString())));
            return encpwd;
        }

        public static Guid RegisterNewUser(string name, string username, string pwd)
        {
            username = username.ToLower();
            Guid g = Guid.NewGuid();
            pwd = ComputePwd(username, pwd);


            using (var db = TaskPadDbContext.Get())
            {
                
                var usr = (from z in db.Users
                           where z.Email.Equals(username)
                           select z).FirstOrDefault();
                if (usr != null)
                {
                    throw new ApplicationException("User already exists !");
                }


                usr = new User() { Guid = g, Name = name, Email = username, Password = pwd };
                db.Users.Add(usr);
                db.SaveChanges();

                return g;
            }

        }

        public static User GetUser(string username)
        {
            username = username.ToLower();

            using (var db = TaskPadDbContext.Get())
            {
                var usr = (from z in db.Users
                           where z.Email.Equals(username)
                           select z).FirstOrDefault();
                
                if (usr != null)
                {
                    usr.Password = null;
                    return usr;
                }
            }

            return null;
        }


        public static void RefreshAccounts(Guid userId, IEnumerable<VsoApi.Profiles.Account> accounts)
        {
            using (var db = TaskPadDbContext.Get())
            {

                var knowns = (from z in db.UserVsoAccounts
                           where z.UserGuid.Equals(userId)
                           select z).ToList();

                var names = (from z in knowns
                             select z.Name).ToList();

                var lesNew = (from z in accounts
                              where !names.Contains(z.name)
                              select z).ToList();
                var lesBoth = (from z in accounts
                              where names.Contains(z.name)
                              select z).ToList();
                
                names = (from z in accounts
                         select z.name).ToList();

                var lesDels = (from z in knowns
                               where !names.Contains(z.Name)
                               select z).ToList();

                foreach(var l in lesDels)
                {
                    db.Projects.RemoveRange(from z in db.Projects
                                             where z.AccountGuid.Equals(l.Guid)
                                             select z);
                    db.UserVsoAccounts.Remove(l);
                }


                foreach(var l in lesNew)
                {
                    UserVsoAccount acc = new UserVsoAccount()
                    {
                        Guid = l.id,
                        Label = l.name,
                        UserGuid = userId,
                        Name = l.name
                    };
                    db.UserVsoAccounts.Add(acc);
                } 

                foreach(var l in lesBoth)
                {
                    var old = (from z in knowns
                               where z.Name.Equals(l.name)
                               select z).FirstOrDefault();
                    if (old == null)
                        continue;
                    
                    old.Label = l.name;
                }

                db.SaveChanges();
            }


        }
    }
}
