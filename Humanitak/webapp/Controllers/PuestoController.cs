using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class PuestoController : Controller
    {
        // GET: Puesto
        public ActionResult Index()
        {
            return View();
        }
        // GET: Nuevo
        public ActionResult Nuevo()
        {
            return View();
        }
    }
}