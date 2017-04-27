using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class NominaEmpleadoController : Controller
    {
        // GET: NominaEmpleado
        public ActionResult Index()
        {
            Session["Empresa"] = null;
            return View();
        }
    }
}