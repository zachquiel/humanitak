using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using SmartAdminMvc.App_Helpers;
using SmartAdminMvc.Extensions;
using SmartAdminMvc.Models;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Controllers {
    public class PercepcionesController : Controller {
        // GET: Percepciones
        public ActionResult Index(int id) {
            using (var db = new DataContext())
                return View(db.Enterprises.First(e => e.Id == id).ToPerceptionListViewModel());
        }

        // GET: Nuevo
        public PartialViewResult _Upsert(int perceptionId, int enterpriseId) {
            if (perceptionId == 0) return PartialView(new PerceptionViewModel {EnterpriseId = enterpriseId});
            using (var db = new DataContext()) {
                var vm = db.Perceptions.First(e => e.Id == perceptionId).ToPerceptionViewModel(enterpriseId);
                return PartialView(vm);
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _Upsert(PerceptionViewModel viewModel) {
            if (!ModelState.IsValid || viewModel == null) return _Upsert(0, 0);
            viewModel.Processed = true;

            if (viewModel.Id == 0) {
                //new
                using (var db = new DataContext()) {
                    var parent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                    var per = new Perception {
                        KeyName = viewModel.KeyName,
                        Description = viewModel.Description,
                        Formula = viewModel.Formula
                    };
                    try {
                        db.Perceptions.AddOrUpdate(per);
                        parent.Perceptions.Add(per);
                        db.SaveChanges();
                        viewModel.Success = true;
                        viewModel.ProcessedMessage = "La percepción fue insertada con éxito";
                    }
                    catch (Exception e) {
                        viewModel.Success = false;
                        viewModel.ProcessedMessage = e.Message;
                    }
                    return PartialView(viewModel);
                }
            }
            using (var db = new DataContext()) {
                var per = db.Perceptions.First(e => e.Id == viewModel.Id);
                per.KeyName = viewModel.KeyName;
                per.Description = viewModel.Description;
                per.Formula = viewModel.Formula;
                try {
                    db.Perceptions.AddOrUpdate(per);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "La percepción fue modificada con éxito";
                }
                catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                return PartialView(viewModel);
            }
        }

        public PartialViewResult _Delete(int perceptionId, int enterpriseId) {
            using (var db = new DataContext())
                return PartialView(db.Perceptions.First(e => e.Id == perceptionId).ToPerceptionViewModel(enterpriseId));
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _Delete(PerceptionViewModel viewModel) {
            if (viewModel == null) return PartialView(viewModel);
            viewModel.Processed = true;
            using (var db = new DataContext()) {
                var per = db.Perceptions.First(e => e.Id == viewModel.Id);
                var ent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                try {
                    ent.Perceptions.Remove(per);
                    db.Perceptions.Remove(per);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "La percepción fue eliminada con éxito";
                }
                catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                return PartialView(viewModel);
            }
        }

        // GET: Nuevo
        public PartialViewResult _AddSpecial(int perceptionId, int enterpriseId, bool group, bool department = false) {
            using (var db = new DataContext()) {
                var gps = db.Groups.Select(g => new GroupReferenceViewModel {
                    Id = g.Id,
                    Name = g.Name,
                }).ToList();
                var dps = db.Departments.Select(g => new DepartmentReferenceViewModel {
                    Id = g.Id,
                    Name = g.Name,
                }).ToList();
                var per =
                    db.Perceptions.Where(g => string.IsNullOrEmpty(g.Formula))
                        .Select(g => new PerceptionReferenceViewModel {
                            Id = g.Id,
                            Description = g.Description
                        }).ToList();
                if (perceptionId == 0)
                    return PartialView(new SpecialPerceptionInsertViewModel {
                        EnterpriseId = enterpriseId,
                        Groups = gps,
                        Departments = dps,
                        Perceptions = per,
                        ShowGroups = group,
                        ShowDepartments = department
                    });
                var vm = db.SpecialPerceptions.First(e => e.Id == perceptionId)
                    .ToSpecialPerceptionInsertViewModel(gps, per, enterpriseId);
                vm.ShowGroups = group;
                vm.ShowDepartments = department;
                return PartialView(vm);
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _AddSpecial(SpecialPerceptionInsertViewModel viewModel) {
            if (!ModelState.IsValid || viewModel == null) return _AddSpecial(0, 0, false);
            viewModel.Processed = true;
            using (var db = new DataContext()) {
                var parent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                var per = db.Perceptions.First(e => e.Id == viewModel.PerceptionId);
                var sp = new SpecialPerception {
                    Amount = viewModel.Amount,
                    Perception = per,
                    Repeat =
                        (viewModel.Permanent != null && viewModel.Permanent.ToLowerInvariant() == "on")
                            ? 0
                            : viewModel.Repeat
                };
                try {
                    db.SpecialPerceptions.AddOrUpdate(sp);
                    if (viewModel.GroupId > 0) {
                        var gp = db.Groups.First(g => g.Id == viewModel.GroupId);
                        sp.Group = gp;
                        gp.SpecialPerceptions.Add(sp);
                    }
                    if (viewModel.DepartmentId > 0) {
                        var dp = db.Departments.First(g => g.Id == viewModel.DepartmentId);
                        sp.Department = dp;
                        dp.SpecialPerceptions.Add(sp);
                    }
                    parent.SpecialPerceptions.Add(sp);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "La percepción fue insertada con éxito";
                }
                catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                return PartialView(viewModel);
            }
        }

        public PartialViewResult _DeleteSpecial(int perceptionId, int enterpriseId) {
            using (var db = new DataContext())
                return
                    PartialView(
                        db.SpecialPerceptions.First(e => e.Id == perceptionId)
                            .ToSpecialPerceptionViewModel(enterpriseId));
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _DeleteSpecial(SpecialPerceptionViewModel viewModel) {
            if (viewModel == null) return PartialView(viewModel);
            viewModel.Processed = true;
            using (var db = new DataContext()) {
                var per = db.SpecialPerceptions.First(e => e.Id == viewModel.Id);
                var ent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                try {
                    ent.SpecialPerceptions.Remove(per);
                    db.SpecialPerceptions.Remove(per);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "La percepción fue eliminada con éxito";
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