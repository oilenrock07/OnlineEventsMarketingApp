using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineEventsMarketingApp.Entities.Users
{
    [Table("AspNetRoles")]
    public class Role
    {
        [Key]
        [StringLength(250)]
        public string Id { get; set; }

        [StringLength(250)]
        public string Name { get; set; }
    }
}
