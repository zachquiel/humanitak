using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class Incidence {
        [Key]
        public long Id { get; set; }

        public virtual Enterprise Enterprise { get; set; }
        public virtual Employee Employee { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int ExtraHours { get; set; }
    }
}