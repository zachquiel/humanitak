namespace SmartAdminMvc.ViewModels {
    public class SalaryComponentsViewModel {
        public double WantedSalary { get; set; }
        public int TotalDays { get; set; }
        public string HasImss { get; set; }

        public double FixedIsrDeduction { get; set; }
        public double PctIsrDeduction { get; set; }
        public double IsrDeductionSubsidy { get; set; }
        public double MonthlyImss { get; set; }
        public double BimonthlyImss { get; set; }
        public double ProratedMonthlyImss { get; set; }
        public double ProratedBimonthlyImss { get; set; }
        public double Divider { get; set; }
        public double GrossSalary { get; set; }
        public double Imss => (BimonthlyImss / 2) + MonthlyImss;
        public double ProratedImss => ProratedMonthlyImss + ProratedBimonthlyImss;
        public double DailySalary => GrossSalary / Divider;
        public double MonthlyTotal => FixedIsrDeduction + PctIsrDeduction - IsrDeductionSubsidy + MonthlyImss;

        public double BimonthlyTotal
            => FixedIsrDeduction + PctIsrDeduction - IsrDeductionSubsidy + MonthlyImss + BimonthlyImss;

        public double MonthlyDaily
            => (FixedIsrDeduction + PctIsrDeduction - IsrDeductionSubsidy + MonthlyImss) / Divider;

        public double BimonthlyDaily
            => (FixedIsrDeduction + PctIsrDeduction - IsrDeductionSubsidy + MonthlyImss + BimonthlyImss) / (Divider * 2);

    }
}