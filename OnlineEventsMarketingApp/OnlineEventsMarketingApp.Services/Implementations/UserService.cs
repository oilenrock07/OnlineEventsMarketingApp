using System.Collections.Generic;
using System.Linq;
using OnlineEventsMarketingApp.Entities.Users;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Services.DataTransferObjects;
using OnlineEventsMarketingApp.Services.Interfaces;

namespace OnlineEventsMarketingApp.Services.Implementations
{
    public class UserService : BaseRepository, IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IDatabaseFactory databaseFactory, IRepository<User> userRepository)
            : base(databaseFactory)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<UserDTO> GetUsers()
        {
            return from userrole in Context.UserRoles.Where(x => x.RoleId != "1")
                    join user in Context.Users on userrole.UserId equals user.Id
                    join role in Context.Roles on userrole.RoleId equals role.Id
                    select new UserDTO
                    {
                        User = user,
                        Role = role.Name
                    };
        }

        public UserDTO GetUserAndRoleId(string userId)
        {
            return (from userrole in Context.UserRoles
                   join user in Context.Users on userrole.UserId equals user.Id
                   join role in Context.Roles on userrole.RoleId equals role.Id
                   where user.Id == userId
                   select new UserDTO
                   {
                       User = user,
                       RoleId = role.Id,
                       Role = role.Name
                   }).FirstOrDefault();
        }

        public User GetByUserId(string id)
        {
            return _userRepository.FirstOrDefault(x => x.Id == id);
        }
    }
}
