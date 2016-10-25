using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.ViewModels {
    public class AccountResetPasswordViewModel {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }
    }
}