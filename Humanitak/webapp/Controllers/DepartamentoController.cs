using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class DepartamentoController : Controller
    {
        // GET: Departamento
        public ActionResult Index()
        {
            return View();
        }

        // GET: Nuevo
        public ActionResult Nuevo()
        {
            return View();
        }

        // GET: Percepcion
        public ActionResult Percepcion()
        {
            return View();
        }
    }
}