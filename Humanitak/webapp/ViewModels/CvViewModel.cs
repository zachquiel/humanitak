namespace SmartAdminMvc.ViewModels {
    public class CvViewModel {
        public string Email_cv { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string FileName { get; set; }
        public bool Processed { get; set; }
        public bool Success { get; set; }
        public string ProcessedMessage { get; set; }
    }
}