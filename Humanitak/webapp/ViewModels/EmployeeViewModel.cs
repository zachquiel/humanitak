using System;
using System.Collections.Generic;

namespace SmartAdminMvc.ViewModels {
    public class EmployeeViewModel : EmployeeReferenceViewModel {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Ssn { get; set; }
        public string Curp { get; set; }
        public string Rfc { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public long PositionId { get; set; }
        public string PositionName { get; set; }
        public long GroupId { get; set; }
        public string GroupName { get; set; }
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
        public string HasSocialSecurity { get; set; }
        public DateTime DoB { get; set; }
        public DateTime SsRegistrationDate { get; set; }
        public string PlaceOfBirth { get; set; }
        public string IdNumber { get; set; }
        public string MaritalStatus { get; set; }
        public long PayingEnterpriseId { get; set; }
        public string PayingEnterpriseName { get; set; }
        public DateTime StartContractDate { get; set; }
        public DateTime? EndContractDate { get; set; }
        public DateTime? PermanentContractDate { get; set; }
        public List<GroupReferenceViewModel> Groups { get; set; }
        public List<EnterpriseReference> Enterprises { get; set; }
        public List<DepartmentReferenceViewModel> Departments { get; set; }
        public List<PositionViewModel> Positions { get; set; }
    }
}