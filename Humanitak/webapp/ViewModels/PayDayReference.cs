namespace SmartAdminMvc.ViewModels {
    public class PayDayReference {
        public long Id { get; set; }
        public string Date { get; set; }
        public bool Processed { get; set; }
        public bool Success { get; set; }
        public string ProcessedMessage { get; set; }
    }
}