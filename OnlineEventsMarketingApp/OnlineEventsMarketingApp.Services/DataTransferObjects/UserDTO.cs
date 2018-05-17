using OnlineEventsMarketingApp.Entities.Users;

namespace OnlineEventsMarketingApp.Services.DataTransferObjects
{
    public class UserDTO
    {
        public User User { get; set; }

        public bool IsEnabled
        {
            get
            {
                if (User != null)
                    return !User.IsDeleted;
                return false;
            }
        }

        public string Role { get; set; }
    }
}
