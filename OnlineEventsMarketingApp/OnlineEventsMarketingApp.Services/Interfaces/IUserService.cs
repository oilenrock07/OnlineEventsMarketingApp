using System.Collections.Generic;
using OnlineEventsMarketingApp.Entities.Users;
using OnlineEventsMarketingApp.Services.DataTransferObjects;

namespace OnlineEventsMarketingApp.Services.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserDTO> GetUsers();

        User GetByUserId(string id);
    }
}
