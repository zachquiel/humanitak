using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models
{
    public class FiscalInformation
    {
        [Key]
        public long Id { get; set; }

        public string StreetAddress { get; set; }
        public string OuterNumeral { get; set; }
        public string InnerNumeral { get; set; }
        public string Area { get; set; }
        public string ZipCode { get; set; }
        public string Town { get; set; }
        public string Rfc { get; set; }
    }
}