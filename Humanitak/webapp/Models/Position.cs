using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class Position {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
    }
}