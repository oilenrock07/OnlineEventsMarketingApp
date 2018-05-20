using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineEventsMarketingApp.Entities.Users
{
    [Table("AspNetUserRoles")]
    public class UserRole
    {
        [Key, Column(Order = 0)]
        [StringLength(250)]
        public string UserId { get; set; }


        [Key, Column(Order = 1)]
        [StringLength(250)]
        public string RoleId { get; set; }
    }
}
