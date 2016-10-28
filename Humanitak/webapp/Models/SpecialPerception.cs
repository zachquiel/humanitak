using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class SpecialPerception {
        [Key]
        public long Id { get; set; }

        public virtual Perception Perception { get; set; }
        public double Amount { get; set; }
        public int Repeat { get; set; }
        public virtual Group Group { get; set; }
        public virtual Department Department { get; set; }
    }
}