using System;

namespace SmartAdminMvc.ViewModels {
    public class IncidenceViewModel {
        public long Id { get; set; }

        public EnterpriseReference Enterprise { get; set; }
        public long EnterpriseId { get; set; }
        public EmployeeReferenceViewModel Employee { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public string StringDate { get; set; }
        public string Type { get; set; }
        public string ExtraHours { get; set; }
        public int TypeIndex { get; set; }
    }
}