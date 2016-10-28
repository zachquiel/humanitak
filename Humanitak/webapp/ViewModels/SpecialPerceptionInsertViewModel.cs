using System.Collections.Generic;

namespace SmartAdminMvc.ViewModels {
    public class SpecialPerceptionInsertViewModel : SpecialPerceptionViewModel {
        public List<PerceptionReferenceViewModel> Perceptions{ get; set; }
        public List<GroupReferenceViewModel> Groups { get; set; }
        public List<DepartmentReferenceViewModel> Departments { get; set; }
        public string Permanent { get; set; }
        public bool ShowGroups { get; set; }
        public bool ShowDepartments { get; set; }
    }
}