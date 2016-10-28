namespace SmartAdminMvc.ViewModels {
    public class DepartmentViewModel : DepartmentReferenceViewModel {
        public long EnterpriseId { get; set; }
        public string Criteria { get; set; }
        public bool Overtime { get; set; }
        public int OvertimeThreshold { get; set; }
        public int DoubleTimeHours { get; set; }
        public bool Processed { get; set; }
        public bool Success { get; set; }
        public string ProcessedMessage { get; set; }
        public int EmployeeCount { get; set; }
    }
}