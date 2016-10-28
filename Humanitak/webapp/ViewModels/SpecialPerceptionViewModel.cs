namespace SmartAdminMvc.ViewModels {
    public class SpecialPerceptionViewModel : PerceptionViewModel {
        public double Amount { get; set; }
        public int Repeat { get; set; }
        public long PerceptionId { get; set; }
        public GroupReferenceViewModel Group { get; set; }
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public DepartmentReferenceViewModel Department { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}