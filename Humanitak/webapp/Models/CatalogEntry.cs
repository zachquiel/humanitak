using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models
{
    public class CatalogEntry
    {
        [Key]
        public long Id { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
    }
}