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

    public class EmployeePayDayFullInfo : PayDayDetailViewModel {
        public string Vacations { get; set; }
        public string BreakDays { get; set; }
        public string Holidays { get; set; }
        public string DoublePay { get; set; }
        public string TriplePay { get; set; }
        public string Overtime { get; set; }
        public string SundayPrime { get; set; }
        public string VacationPrime { get; set; }
        public string IsrDeduction { get; set; }
        public string ImssDeduction { get; set; }
    }
}