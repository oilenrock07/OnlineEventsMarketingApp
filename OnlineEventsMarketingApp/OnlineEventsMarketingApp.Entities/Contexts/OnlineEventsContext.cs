using System.Configuration;
using System.Data.Entity;
using OnlineEventsMarketingApp.Entities.Users;

namespace OnlineEventsMarketingApp.Entities.Contexts
{
    public class OnlineEventsContext : DbContext
    {
        public OnlineEventsContext()
            : base(ConnectionString)
        {

        }

        public virtual IDbSet<Tag> Tags { get; set; }
        public virtual IDbSet<DataSheet> DataSheets { get; set; }
        public virtual IDbSet<NewUserMTD> NewUserMTDs { get; set; }

        //Users
        public virtual IDbSet<Role> Roles { get; set; }
        public virtual IDbSet<User> Users { get; set; }
        public virtual IDbSet<UserClaim> UserClaims { get; set; }
        public virtual IDbSet<UserLogin> UserLogIns { get; set; }
        public virtual IDbSet<UserRole> UserRoles { get; set; }

        static string ConnectionString
        {
            get
            {
                string cs = "";
                switch (ConfigurationManager.AppSettings["DatabaseType"])
                {
                    case "MsSql":
                        cs = "ConnectionString.MsSql";
                        break;
                    case "MySql":
                        cs = "ConnectionString.MySql";
                        break;
                }

                return cs;
            }
        }
    }
}
