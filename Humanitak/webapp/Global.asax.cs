#region Using

using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Configuration = SmartAdminMvc.Migrations.Configuration;

#endregion

namespace SmartAdminMvc {
    public class MvcApplication : HttpApplication {
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"];
            if (connectionString != null) {
                var configuration = new Configuration {
                    TargetDatabase =
                        new DbConnectionInfo(connectionString.ConnectionString, connectionString.ProviderName)
                };
                var migrator = new DbMigrator(configuration);
                migrator.Update();
            }
            IdentityConfig.RegisterIdentities();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}