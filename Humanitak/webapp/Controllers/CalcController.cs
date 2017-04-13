using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Helpers;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Controllers
{
    public class CalcController : Controller {
        // GET: Calc
        [HttpGet]
        public ActionResult Index() {
            return View(new SalaryComponentsViewModel {
                IsrDeductionSubsidy = 0,
                MonthlyImss = 0,
                Divider = 1,
                FixedIsrDeduction = 0,
                PctIsrDeduction = 0,
                BimonthlyImss = 0,
                GrossSalary = 0,
            });
        }

        [HttpPost]
        public PartialViewResult _CalcData(SalaryComponentsViewModel model) {
            return PartialView("_CalcData", DeductionHelper.GetCalculatedSalaryValues(model.WantedSalary, model.TotalDays, model.HasImss == "on"));
        }

        [HttpPost]
        public PartialViewResult _CalcData2(SalaryComponentsViewModel model) {
            return PartialView(DeductionHelper.GetStraightCalculatedSalaryValues(model.WantedSalary, model.TotalDays, model.HasImss == "on"));
        }
        
    }

    public class CalcObject {
        public double WantedSalary { get; set; }
        public int TotalDays { get; set; }
    }
}