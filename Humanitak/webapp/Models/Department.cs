using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class Department {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
        public string Criteria { get; set; }
        public bool Overtime { get; set; }
        public int OvertimeThreshold { get; set; }
        public int DoubleTimeHours { get; set; }
        public virtual List<SpecialPerception> SpecialPerceptions { get; set; }
        public virtual List<Employee> Employees { get; set; }
    }
}