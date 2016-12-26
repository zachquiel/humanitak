using System.Collections.Generic;
using SmartAdminMvc.Models;

namespace SmartAdminMvc.ViewModels {
    public class EnterpriseInfoViewModel : EnterpriseReference {
        public List<EmployeeViewModel> Employees { get; set; }
        public List<DepartmentViewModel> Departments { get; set; }
        public List<PerceptionViewModel> Perceptions { get; set; }
        public List<SpecialPerceptionViewModel> SpecialPerceptions { get; set; }
        public List<PositionViewModel> Positions { get; set; }
        public List<PayDayViewModel> PayDays { get; set; }
        public List<IncidenceViewModel> Incidences { get; set; }
    }
}