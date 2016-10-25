using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class Enterprise {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
        public int Payday1Start { get; set; }
        public int Payday1End { get; set; }
        public int Payday2Start { get; set; }
        public int Payday2End { get; set; }
        public virtual EnterpriseImage Logo { get; set; }
        public virtual EnterpriseImage Header { get; set; }
        public bool UsesPunchClock { get; set; }
        public int Commission { get; set; }
        public double Vat { get; set; }
        public string State { get; set; }
        public bool IsActive { get; set; }
        public virtual Enterprise ParentEnterprise { get; set; }
        public string Operations { get; set; }
        public string City { get; set; }
        public DateTime LastPayday { get; set; }
    }
}