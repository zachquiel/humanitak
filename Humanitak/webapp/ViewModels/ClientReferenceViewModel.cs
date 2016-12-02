using System.Collections.Generic;

namespace SmartAdminMvc.ViewModels {
    public class ClientReferenceViewModel : ClientReference {
        public long EnterpriseId { get; set; }
        public List<EnterpriseViewModel> Enterprises { get; set; }
    }
}