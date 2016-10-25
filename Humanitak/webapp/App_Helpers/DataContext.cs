using System.Data.Entity;
using SmartAdminMvc.Models;

namespace SmartAdminMvc.App_Helpers {
    public class DataContext : DbContext {
        public DataContext() : base("name=DefaultConnection") {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<EnterpriseImage> EnterpriseImages { get; set; }
    }
}