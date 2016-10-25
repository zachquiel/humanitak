namespace SmartAdminMvc.ViewModels {
    public class EnterpriseReference {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Processed { get; set; }
        public bool Success { get; set; }
        public string ProcessedMessage { get; set; }
    }
}