namespace SmartAdminMvc.ViewModels {
    public class EmployeeReferenceViewModel {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Processed { get; set; }
        public bool Success { get; set; }
        public string ProcessedMessage { get; set; }
        public long EnterpriseId { get; set; }
    }
}