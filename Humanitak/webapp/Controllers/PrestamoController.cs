﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class PrestamoController : Controller
    {
        // GET: Prestamo
        public ActionResult Index()
        {
            Session["Empresa"] = null;
            return View();
        }
        public ActionResult Detalle()
        {
            return View();
        }
    }
}