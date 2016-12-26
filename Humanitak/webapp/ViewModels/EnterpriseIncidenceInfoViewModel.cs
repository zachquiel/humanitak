using System.Collections.Generic;

namespace SmartAdminMvc.ViewModels {
    public class EnterpriseIncidenceInfoViewModel : EnterpriseReference {
        public List<IncidenceViewModel> Incidences { get; set; }
        public List<EmployeeViewModel> Employees { get; set; }
        public List<IncidenceViewModel> InsertedIncidences { get; set; }
        public List<IncidenceViewModel> DeletedIncidences { get; set; }
    }
}