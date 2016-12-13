using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class EmployeePayDay {
        [Key]
        public long Id { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual PayDay PayDay { get; set; }
        public int NaturalDays { get; set; }
        public double DailySalary { get; set; }
        public double Vacations { get; set; }
        public double BreakDays { get; set; }
        public double Holidays { get; set; }
        public double DoublePay { get; set; }
        public double TriplePay { get; set; }
        public double Overtime { get; set; }
        public double SundayPrime { get; set; }
        public double VacationPrime { get; set; }
        public double Perceptions { get; set; }
        public double Deductions { get; set; }
    }
}