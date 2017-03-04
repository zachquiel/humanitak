using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class Employee {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
        public string LastName { get; set; }
        public string Ssn { get; set; }
        public string Curp { get; set; }
        public string Rfc { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public virtual Department Department { get; set; }
        public virtual Position Position { get; set; }
        public virtual Group Group { get; set; }
        public double DailySalary { get; set; }
        public DateTime StartDate { get; set; }
        public string Bank { get; set; }
        public string AccountNumber { get; set; }
        public string OffDays { get; set; }
        public bool IsActive { get; set; }
        public string Address { get; set; }
        public string Area { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public bool HasSocialSecurity { get; set; }
        public DateTime DoB { get; set; }
        public DateTime SsRegistrationDate { get; set; }
        public string PlaceOfBirth { get; set; }
        public string IdNumber { get; set; }
        public string MaritalStatus { get; set; }
        public virtual Enterprise PayingEnterprise { get; set; }
        public virtual Enterprise SecondaryEnterprise { get; set; }
        public DateTime StartContractDate { get; set; }
        public DateTime? EndContractDate { get; set; }
        public DateTime? PermanentContractDate { get; set; }
        public string WorkState { get; set; }
        public string PatronalRegistryNo { get; set; }
        public string Regime { get; set; }
        public int PaymentFrequency { get; set; }
    }
}