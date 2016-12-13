using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAdminMvc.Models {
    public class PayDay {
        [Key]
        public long Id { get; set; }

        public DateTime PayStartDate { get; set; }
        public DateTime PayDate { get; set; }
        public DateTime AuthotizationDate { get; set; }
        public virtual User Creator { get; set; }
        public virtual User Authorizer { get; set; }
        public virtual Enterprise Enterprise { get; set; }
    }
}