using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class EnterpriseImage {
        [Key]
        public long Id { get; set; }

        public byte[] Image { get; set; }
    }
}