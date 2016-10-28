using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using SmartAdminMvc.App_Helpers;
using SmartAdminMvc.Extensions;
using SmartAdminMvc.Models;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Controllers {
    public class DepartamentoController : Controller {
        // GET: Departamento
        public ActionResult Index(int id) {
            using (var db = new DataContext())
                return View(db.Enterprises.First(e => e.Id == id).ToDepartmentListViewModel());
        }

        // GET: Nuevo
        public PartialViewResult _Upsert(int departmentId, int enterpriseId) {
            if (departmentId == 0) return PartialView(new DepartmentViewModel {EnterpriseId = enterpriseId});
            using (var db = new DataContext()) {
                var vm = db.Departments.First(e => e.Id == departmentId).ToDepartmentViewModel(enterpriseId);
                return PartialView(vm);
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _Upsert(DepartmentViewModel viewModel) {
            if (!ModelState.IsValid || viewModel == null) return _Upsert(0, 0);
            viewModel.Processed = true;

            if (viewModel.Id == 0) {
                //new
                using (var db = new DataContext()) {
                    var parent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                    var dep = new Department {
                        Name = viewModel.Name,
                        Criteria = viewModel.Criteria,
                        DoubleTimeHours = viewModel.DoubleTimeHours,
                        Overtime = viewModel.Overtime,
                        OvertimeThreshold = viewModel.OvertimeThreshold
                    };
                    try {
                        db.Departments.AddOrUpdate(dep);
                        parent.Departments.Add(dep);
                        db.SaveChanges();
                        viewModel.Success = true;
                        viewModel.ProcessedMessage = "El departamento fue insertado con éxito";
                    }
                    catch (Exception e) {
                        viewModel.Success = false;
                        viewModel.ProcessedMessage = e.Message;
                    }
                    return PartialView(viewModel);
                }
            }
            using (var db = new DataContext()) {
                var dep = db.Departments.First(e => e.Id == viewModel.Id);
                dep.Name = viewModel.Name;
                dep.Criteria = viewModel.Criteria;
                dep.DoubleTimeHours = viewModel.DoubleTimeHours;
                dep.Overtime = viewModel.Overtime;
                dep.OvertimeThreshold = viewModel.OvertimeThreshold;
                try {
                    db.Departments.AddOrUpdate(dep);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "El departamento fue modificado con éxito";
                }
                catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                return PartialView(viewModel);
            }
        }

        public PartialViewResult _Delete(int departmentId, int enterpriseId) {
            using (var db = new DataContext())
                return PartialView(db.Departments.First(e => e.Id == departmentId).ToDepartmentViewModel(enterpriseId));
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _Delete(DepartmentViewModel viewModel) {
            if (viewModel == null) return PartialView(viewModel);
            viewModel.Processed = true;
            using (var db = new DataContext()) {
                var dep = db.Departments.First(e => e.Id == viewModel.Id);
                var ent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                try {
                    ent.Departments.Remove(dep);
                    db.Departments.Remove(dep);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "El departamento fue eliminado con éxito";
                }
                catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                return PartialView(viewModel);
            }
        }
    }
}