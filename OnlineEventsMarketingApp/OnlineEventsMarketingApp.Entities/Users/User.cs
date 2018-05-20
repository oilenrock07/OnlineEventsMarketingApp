using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineEventsMarketingApp.Entities.Users
{
    [Table("AspNetUsers")]
    public class User
    {
        [Key]
        public string Id { get; set; }

        public string Email { get; set; }

        [StringLength(250)]
        public string UserName { get; set; }
        
        [StringLength(500)]
        public string PasswordHash { get; set; }
        
        [StringLength(500)]
        public string SecurityStamp { get; set; }
        
        //[StringLength(500)]
        //public string Discriminator { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public bool IsDeleted { get; set; }
    }
}
