using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;

namespace SmartAdminMvc.Models {
    public class User : IUser<string> {
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public DateTime LastAccess { get; set; }

        public string UserType { get; set; }

        public bool IsActive { get; set; }

        [Key]
        public string Id { get; set; }


        public string UserName { get; set; }
        public string BusinessType { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool CanIssuePayments { get; set; }
    }
}