using System;
using System.ComponentModel.DataAnnotations;
using SmartAdminMvc.Models;

namespace SmartAdminMvc.ViewModels {
    public class AccountRegistrationViewModel {
        public string UserType { get; set; }
        public long LinkedEnterprise { get; set; }
        public string LinkedEnterpriseName { get; set; }

        public string FirstName { get; set; }
        public string UserName { get; set; }

        public string LastName { get; set; }

        public string CanIssuePayments { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }

        public DateTime LastAccess { get; set; }

        public bool IsActive { get; set; }

        public string Id { get; set; }


        public bool Processed { get; set; }

        public bool Success { get; set; }

        public string ProcessedMessage { get; set; }

        public ClientReference[] EnterpriseCatalog { get; set; }
    }
}