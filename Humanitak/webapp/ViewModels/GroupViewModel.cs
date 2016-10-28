using System.Collections.Generic;

namespace SmartAdminMvc.ViewModels {
    public class GroupViewModel : GroupReferenceViewModel {
        public virtual List<EmployeeReferenceViewModel> Employees { get; set; }
    }
}