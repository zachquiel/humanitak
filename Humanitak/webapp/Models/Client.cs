﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class Client {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
        public string LegalRepresentative { get; set; }
        public int Payday1Start { get; set; }
        public int Payday1End { get; set; }
        public int Payday2Start { get; set; }
        public int Payday2End { get; set; }
        public virtual EnterpriseImage Logo { get; set; }
        public virtual EnterpriseImage Header { get; set; }
        public bool UsesPunchClock { get; set; }
        public double Commission { get; set; }
        public double Vat { get; set; }
        public string State { get; set; }
        public bool IsActive { get; set; }
        public string Operations { get; set; }
        public string City { get; set; }
        public DateTime LastPayday { get; set; }
        public virtual FiscalInformation FiscalInformation { get; set; }
        public virtual List<Enterprise> Enterprises { get; set; }
        public virtual List<Position> Positions { get; set; }
        public virtual List<Department> Departments { get; set; }
        public virtual List<Perception> Perceptions { get; set; }
        public virtual List<SpecialPerception> SpecialPerceptions { get; set; }
        public virtual List<Employee> Employees { get; set; }
        public virtual List<Group> Groups { get; set; }
    }
}