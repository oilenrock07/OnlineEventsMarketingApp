using System.Collections.Generic;
using OnlineEventsMarketingApp.Interfaces;
using OnlineEventsMarketingApp.Services.DataTransferObjects;

namespace OnlineEventsMarketingApp.Models.Accounts
{
    public class UserListViewModel
    {
        public IPaginationModel Pagination { get; set; }
        public IEnumerable<UserDTO> Users { get; set; }
    }
}