namespace SmartAdminMvc.ViewModels {
    public class PayDayViewModel : PayDayReference {
        public string StartDate { get; set; }
        public string Authorized { get; set; }
        public int Employees { get; set; }
        public string Authorizer { get; set; }
        public string AuthorizationDate { get; set; }
        public long EnterpriseId { get; set; }
    }
}