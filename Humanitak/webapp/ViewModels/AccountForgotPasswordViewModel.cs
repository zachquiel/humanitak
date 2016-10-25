using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.ViewModels {
    public class AccountForgotPasswordViewModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}