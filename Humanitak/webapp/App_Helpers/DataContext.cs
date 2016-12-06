using System.Data.Entity;
using SmartAdminMvc.Models;

namespace SmartAdminMvc.App_Helpers {
    public class DataContext : DbContext {
        public DataContext() : base("name=DefaultConnection") {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<EnterpriseImage> EnterpriseImages { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Perception> Perceptions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<SpecialPerception> SpecialPerceptions { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<FiscalInformation> FiscalInformations { get; set; }
    }
}