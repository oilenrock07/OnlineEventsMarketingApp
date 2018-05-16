using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineEventsMarketingApp.Entities.Users;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Services.Interfaces;

namespace OnlineEventsMarketingApp.Services.Implementations
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        

        public UserRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public IEnumerable<User> GetUsers()
        {
            return from userrole in Context.UserRoles//.Where(x => x.RoleId != "1")
                    join user in Context.Users on userrole.UserId equals user.Id
                    select user;
        }
    }
}
