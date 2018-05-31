using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineEventsMarketingApp.Entities
{
    public class DataSheet
    {
        [Key]
        public int DataSheetId { get; set; }

        public int DIS { get; set; }

        public int TE { get; set; }

        [StringLength(250)]
        public string TM { get; set; } //sales rep

        [StringLength(500)]
        public string Area { get; set; }

        [StringLength(50)]
        public string InHouse { get; set; }

        [StringLength(250)]
        public string Rnd { get; set; }

        public DateTime Date { get; set; }

        public int NewUsers { get; set; }

        public int ExistingUsers { get; set; }

        [StringLength(250)]
        public string Status { get; set; }

        public int NoOfPatients { get; set; }
        
        public int ? TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }
        //public bool IsDeleted { get; set; }
    }
}
