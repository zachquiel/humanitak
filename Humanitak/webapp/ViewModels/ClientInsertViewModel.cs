﻿using System.Collections.Generic;

namespace SmartAdminMvc.ViewModels {
    public class ClientInsertViewModel : ClientViewModel {
        public List<ClientReference> Clients { get; set; }
        public string StreetAddress { get; set; }
        public string OuterNumeral { get; set; }
        public string InnerNumeral { get; set; }
        public string Area { get; set; }
        public string ZipCode { get; set; }
        public string Town { get; set; }
        public string Rfc { get; set; }
        public long FiscalId { get; set; }
    }
}