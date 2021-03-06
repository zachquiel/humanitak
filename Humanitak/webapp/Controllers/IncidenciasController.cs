﻿using System;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SmartAdminMvc.App_Helpers;
using SmartAdminMvc.Extensions;
using SmartAdminMvc.Models;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Controllers {
    public class IncidenciasController : Controller {
        // GET: Incidencias
        public ActionResult Index(int id) {
            using (var db = new DataContext()) {
                var incidences = db.Incidences.Where(i => i.Enterprise.Id == id).ToList();
                return View(db.Enterprises.First(e => e.Id == id).ToIncidenceInfoViewModel(incidences));
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public long InsertEvent(IncidenceViewModel viewModel) {
            if (viewModel == null) return 0;
            try {
                using (var db = new DataContext()) {
                    var incidences = db.Incidences.ToList();
                    var ent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                    if (viewModel.Id == 0) {
                        var emp = ent.Employees.First(e => (e.Name + " " + e.LastName) == viewModel.EmployeeName);
                        var type = viewModel.Type;
                        var hours = 0;
                        if (viewModel.Type.EndsWith("Horas Extra")) {
                            hours = int.Parse(viewModel.ExtraHours.Split(' ')[0]);
                            type = "Horas Extra";
                        }
                        viewModel.Type = viewModel.Type.Replace("Día", "Dia");
                        var incidenceDate = DateTime.ParseExact(viewModel.StringDate, "dd/MM/yyyy",
                                new CultureInfo("es-MX"))
                            .AddMonths(1)
                            .AddDays(1);
                        if (incidences.Any(i => i.Employee.Id == emp.Id && i.Type == type && i.Date == incidenceDate))
                            return
                                incidences.First(
                                    i => i.Employee.Id == emp.Id && i.Type == type && i.Date == incidenceDate).Id;
                        var inc = new Incidence {
                            Date = incidenceDate,
                            Employee = emp,
                            Enterprise = ent,
                            ExtraHours = hours,
                            Type = type
                        };
                        db.Incidences.AddOrUpdate(inc);
                        db.SaveChanges();
                        return inc.Id;
                    }
                    else {
                        var inc = incidences.First(i => i.Id == viewModel.Id);
                        var date = DateTime.ParseExact(viewModel.StringDate, "dd/MM/yyyy", new CultureInfo("es-MX"))
                            .AddMonths(1)
                            .AddDays(1);
                        if (inc != null && date != inc.Date)
                            inc.Date = date;

                        db.Incidences.AddOrUpdate(inc);
                        db.SaveChanges();
                    }
                    return viewModel.Id;
                }
            }
            catch (Exception e) {
                var valid = ModelState.IsValid;
            }
            return 0;
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public long DeleteEvent(IncidenceViewModel viewModel) {
            if (viewModel == null) return 0;
            try {
                using (var db = new DataContext()) {
                    var incidences = db.Incidences.ToList();
                    if (viewModel.Id == 0 || incidences.All(i => i.Id != viewModel.Id)) return viewModel.Id;
                    var inc = incidences.First(i => i.Id == viewModel.Id);
                    db.Incidences.Remove(inc);
                    db.SaveChanges();
                    return viewModel.Id;
                }
            }
            catch (Exception e) {
                var valid = ModelState.IsValid;
            }
            return 0;
        }
    }

}