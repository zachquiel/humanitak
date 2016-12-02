using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.ViewModels {
    public class ClientViewModel : ClientReference {
        public int Payday1Start { get; set; }
        public int Payday1End { get; set; }
        public int Payday2Start { get; set; }
        public int Payday2End { get; set; }

        [DataType(DataType.Upload)]
        public byte[] LogoImage { get; set; }

        [DataType(DataType.Upload)]
        public byte[] HeaderImage { get; set; }

        public string UsesPunchClock { get; set; }
        public int Commission { get; set; }
        public double Vat { get; set; }
        public string State { get; set; }
        public string IsActive { get; set; }
        public List<EnterpriseViewModel> Enterprises { get; set; }
        public string Operations { get; set; }
        public string City { get; set; }
        public DateTime LastPayday { get; set; }
        public long EnterpriseId { get; set; }
    }
}