using System.Collections.Generic;

namespace SmartAdminMvc.ViewModels {
    public class EnterpriseInfoViewModel : EnterpriseReference {
        public List<EmployeeViewModel> Employees { get; set; }
        public List<DepartmentViewModel> Departments { get; set; }
        public List<PerceptionViewModel> Perceptions { get; set; }
        public List<SpecialPerceptionViewModel> SpecialPerceptions { get; set; }
        public List<PositionViewModel> Positions { get; set; }
    }
}