using System.Collections.Generic;
using OnlineEventsMarketingApp.Entities.Users;

namespace OnlineEventsMarketingApp.Services.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
    }
}
