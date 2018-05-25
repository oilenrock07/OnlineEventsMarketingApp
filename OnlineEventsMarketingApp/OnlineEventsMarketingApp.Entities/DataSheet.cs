using System;
using System.ComponentModel.DataAnnotations;

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

        public byte Status { get; set; }

        public int NoOfPatients { get; set; }

        public int ? TagId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
