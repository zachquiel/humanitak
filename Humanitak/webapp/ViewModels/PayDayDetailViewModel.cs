namespace SmartAdminMvc.ViewModels {
    public class PayDayDetailViewModel {
        public string Name { get; set; }
        public string DailySalary { get; set; }
        public int NaturalDays { get; set; }
        public string Income { get; set; }
        public string Perceptions { get; set; }
        public string Deductions { get; set; }
        public string FinalIncome { get; set; }
        public string Department { get; set; }
    }

    public class PayDayFullInfo : PayDayDetailViewModel {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Commission { get; set; }
        public string Vat { get; set; }
        public string FinalAmount { get; set; }
        public string Status { get; set; }
        public string CommissionPct { get; set; }
        public string VatPct { get; set; }
    }
}