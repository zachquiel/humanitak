using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace SmartAdminMvc.Controllers
{
    [Authorize]
    public class EmpresasController : Controller
    {
        // GET: Nominas
        public ActionResult Nomina()
        {
            return View();
        }

        // GET: Catalogo
        public ActionResult Index()
        {
            return View();
        }

        // GET: Catalogo
        public ActionResult Empresa(int id)
        {
            ViewData.Add("id", id);
            return View();
        }

        // GET: Nueva
        public ActionResult Nueva()
        {
            return View();
        }

        // GET: Nueva
        public ActionResult Empleados()
        {
            return View();
        }

        // GET: Nueva
        public ActionResult Departamentos()
        {
            return View();
        }

        // GET: Nueva
        public ActionResult Puestos()
        {
            return View();
        }

        // GET: Nueva
        public ActionResult Percepciones()
        {
            return View();
        }

        // GET: Nueva
        public ActionResult Configuracion()
        {
            return View();
        }

        // GET: Nueva
        public ActionResult Acumulados()
        {
            return View();
        }
    }
}