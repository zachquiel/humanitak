using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class ResumeInfo {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string FileName { get; set; }
        public string Date { get; set; }

    }
}