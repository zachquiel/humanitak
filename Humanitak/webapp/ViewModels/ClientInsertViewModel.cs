using System.Collections.Generic;

namespace SmartAdminMvc.ViewModels {
    public class ClientInsertViewModel : ClientViewModel {
        public List<ClientReference> Clients { get; set; }
    }
}