#region Using

using System.Web.Mvc;

#endregion

namespace SmartAdminMvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: home/index
        public ActionResult Index() {
            Session["Empresa"] = null;
            return View();
        }
    }
}