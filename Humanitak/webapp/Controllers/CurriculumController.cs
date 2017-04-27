using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.App_Helpers;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Controllers
{
    public class CurriculumController : Controller
    {
        // GET: Curriculum
        public ActionResult Index()
        {
            Session["Empresa"] = null;
            var cvs = new List<CvViewModel>();
            using (var db = new DataContext()) {
                cvs = db.ResumeInfo.Select(r => new CvViewModel {
                    Name = r.Name,
                    Date = r.Date,
                    Email_cv = r.Email,
                    FileName = r.FileName
                }).ToList();
            }
            return View(cvs);
        }
    }
}