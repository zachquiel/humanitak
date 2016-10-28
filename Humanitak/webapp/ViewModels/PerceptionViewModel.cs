namespace SmartAdminMvc.ViewModels {
    public class PerceptionViewModel :PerceptionReferenceViewModel {
        public long EnterpriseId { get; set; }
        public string KeyName { get; set; }
        public string Formula { get; set; }
        public bool Processed { get; set; }
        public bool Success { get; set; }
        public string ProcessedMessage { get; set; }
    }
}