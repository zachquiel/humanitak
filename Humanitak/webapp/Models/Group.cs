using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class Group {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
        public virtual List<Employee> Employees { get; set; }
        public virtual List<SpecialPerception> SpecialPerceptions { get; set; }
    }
}