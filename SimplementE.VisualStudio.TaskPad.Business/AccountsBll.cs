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

        public static void RefreshTokens(string username, string accessToken, string refreshToken)
        {
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

    }
}
