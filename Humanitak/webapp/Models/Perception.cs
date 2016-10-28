using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class Perception {
        [Key]
        public long Id { get; set; }

        public string KeyName { get; set; }
        public string Description { get; set; }
        public string Formula { get; set; }
    }
}