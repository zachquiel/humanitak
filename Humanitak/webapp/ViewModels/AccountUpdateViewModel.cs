﻿#region Using

using System.ComponentModel.DataAnnotations;

#endregion

namespace SmartAdminMvc.ViewModels {
    public class AccountUpdateViewModel {
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

        public bool Processed { get; set; }

        public bool Success { get; set; }

        public string ProcessedMessage { get; set; }

        public ClientReference[] EnterpriseCatalog { get; set; }
    }
}