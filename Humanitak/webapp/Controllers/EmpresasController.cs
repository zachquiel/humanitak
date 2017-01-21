using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.App_Helpers;
using SmartAdminMvc.Extensions;
using SmartAdminMvc.Models;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Controllers {
    [Authorize]
    public class EmpresasController : Controller {
        // GET: Nominas
        public ActionResult Nomina() {
            return View();
        }

        // GET: Catalogo
        public ActionResult Index() {
            var list = new List<EnterpriseViewModel>();
            using (var db = new DataContext()) {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var enterprise in db.Enterprises.ToList()) list.Add(enterprise.ToEnterpriseViewModel());
            }
            return View(list);
        }

        // GET: Catalogo
        public ActionResult Empresa(int id) {
            using (var db = new DataContext()) {
                return View(db.Enterprises.First(e => e.Id == id).ToEnterpriseReference());
            }
        }

        // GET: Nueva
        public ActionResult Nueva() {
            return View();
        }

        // GET: Nueva
        public ActionResult Empleados() {
            return View();
        }

        // GET: Nueva
        public ActionResult Departamentos() {
            return View();
        }

        // GET: Nueva
        public ActionResult Puestos() {
            return View();
        }

        // GET: Nueva
        public ActionResult Percepciones() {
            return View();
        }

        // GET: Nueva
        public ActionResult Configuracion() {
            return View();
        }

        // GET: Nueva
        public ActionResult Acumulados() {
            return View();
        }

        // GET: Nueva
        public ActionResult Dispersion(int id) {
            ViewData.Add("id", id);
            return View();
        }

        public ActionResult GetResult() {
            var message = "Welcome";
            return new JsonResult {Data = message, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }

        public PartialViewResult _Nueva(int id) {
            if (id == 0) {
                using (var db = new DataContext()) {
                    return PartialView(new EnterpriseInsertViewModel {
                        Enterprises =
                            db.Enterprises.Where(e => e.ParentEnterprise == null).Select(e => new EnterpriseReference {
                                Id = e.Id,
                                Name = e.Name
                            }).ToList(),
                        Payday1End = -1,
                        Payday1Start = -1,
                        Payday2End = -1,
                        Payday2Start = -1,
                        State = "0",
                        ParentEnterprise = -1
                    });
                }
            }
            using (var db = new DataContext()) {
                var ents = db.Enterprises.Where(e => e.ParentEnterprise == null).Select(e => new EnterpriseReference {
                    Id = e.Id,
                    Name = e.Name
                }).ToList();
                var vm = db.Enterprises.First(e => e.Id == id).ToEnterpriseInsertViewModel(ents);
                return PartialView(vm);
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _Nueva([Bind(Exclude = "logoImage, headerImage")] EnterpriseInsertViewModel viewModel,
            HttpPostedFileBase logoImage, HttpPostedFileBase headerImage) {
            if (!ModelState.IsValid || viewModel == null) return _Nueva(0);

            byte[] logoImageData = null;
            byte[] headerImageData = null;
            if (Request.Files.Count <= 0 && viewModel.Id == 0) return _Nueva(0);
            var logoImage2 = Request.Files["logoImage"];
            if (logoImage2 == null && viewModel.Id == 0) return _Nueva(0);
            var headerImage2 = Request.Files["headerImage"];
            if (headerImage2 == null && viewModel.Id == 0) return _Nueva(0);
            if (viewModel.Id == 0) {
                //new
                viewModel.Processed = true;
                using (var binaryReader = new BinaryReader(logoImage2.InputStream))
                    logoImageData = binaryReader.ReadBytes(logoImage2.ContentLength);
                using (var binaryReader = new BinaryReader(headerImage2.InputStream))
                    headerImageData = binaryReader.ReadBytes(headerImage2.ContentLength);
                using (var db = new DataContext()) {
                    Enterprise parent = null;
                    if (viewModel.ParentEnterprise != 0)
                        parent = db.Enterprises.First(e => e.Id == viewModel.ParentEnterprise);
                    var ent = new Enterprise {
                        City = viewModel.City,
                        //Commission = viewModel.Commission,
                        Header = new EnterpriseImage {
                            Image = headerImageData
                        },
                        IsActive = true,
                        Logo = new EnterpriseImage {
                            Image = logoImageData
                        },
                        Name = viewModel.Name,
                        Operations = viewModel.Operations,
                        Payday1Start = viewModel.Payday1Start,
                        Payday1End = viewModel.Payday1End,
                        Payday2Start = viewModel.Payday2Start,
                        Payday2End = viewModel.Payday2End,
                        State = viewModel.State,
                        Vat = viewModel.Vat,
                        UsesPunchClock =
                            viewModel.UsesPunchClock != null && viewModel.UsesPunchClock.ToLowerInvariant() == "on",
                        LastPayday = new DateTime(1970, 1, 1),
                        ParentEnterprise = parent ?? parent
                    };
                    try {
                        db.Enterprises.AddOrUpdate(ent);
                        db.SaveChanges();
                        viewModel.Success = true;
                        viewModel.ProcessedMessage = "La empresa fue insertada con éxito";
                    } catch (Exception e) {
                        viewModel.Success = false;
                        viewModel.ProcessedMessage = e.Message;
                    }
                    viewModel.Enterprises =
                        db.Enterprises.Where(e => e.ParentEnterprise == null).Select(e => new EnterpriseReference {
                            Id = e.Id,
                            Name = e.Name
                        }).ToList();
                    return PartialView(viewModel);
                }
            }
            viewModel.Processed = true;
            if (logoImage2 != null) {
                using (var binaryReader = new BinaryReader(logoImage2.InputStream))
                    logoImageData = binaryReader.ReadBytes(logoImage2.ContentLength);
            }
            if (headerImage2 != null) {
                using (var binaryReader = new BinaryReader(headerImage2.InputStream))
                    headerImageData = binaryReader.ReadBytes(headerImage2.ContentLength);
            }
            using (var db = new DataContext()) {
                Enterprise parent = null;
                if (viewModel.ParentEnterprise != 0)
                    parent = db.Enterprises.First(e => e.Id == viewModel.ParentEnterprise);
                var ent = db.Enterprises.First(e => e.Id == viewModel.Id);
                ent.City = viewModel.City;
                //ent.Commission = viewModel.Commission;
                if (headerImageData != null) {
                    ent.Header.Image = headerImageData;
                    db.EnterpriseImages.AddOrUpdate(ent.Header);
                }
                ent.IsActive = true;
                if (logoImageData != null) {
                    ent.Logo.Image = logoImageData;
                    db.EnterpriseImages.AddOrUpdate(ent.Logo);
                }
                ent.Name = viewModel.Name;
                ent.Operations = viewModel.Operations;
                ent.Payday1Start = viewModel.Payday1Start;
                ent.Payday1End = viewModel.Payday1End;
                ent.Payday2Start = viewModel.Payday2Start;
                ent.Payday2End = viewModel.Payday2End;
                ent.State = viewModel.State;
                ent.Vat = viewModel.Vat;
                ent.UsesPunchClock = viewModel.UsesPunchClock != null &&
                                     viewModel.UsesPunchClock.ToLowerInvariant() == "on";
                ent.ParentEnterprise = parent ?? parent;
                try {
                    db.Enterprises.AddOrUpdate(ent);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "La empresa fue modificada con éxito";
                } catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                viewModel.Enterprises =
                    db.Enterprises.Where(e => e.ParentEnterprise == null).Select(e => new EnterpriseReference {
                        Id = e.Id,
                        Name = e.Name
                    }).ToList();
                return PartialView(viewModel);
            }
        }

        public PartialViewResult _Eliminar(int id) {
            using (var db = new DataContext()) {
                return PartialView(db.Enterprises.Where(e => e.Id == id).Select(e => new EnterpriseReference {
                    Id = e.Id,
                    Name = e.Name
                }).First());
            }
        }

        [HttpPost]
        public PartialViewResult _Eliminar(EnterpriseReference viewModel) {
            if (viewModel == null) return PartialView(viewModel);
            viewModel.Processed = true;
            using (var db = new DataContext()) {
                var ent = db.Enterprises.First(e => e.Id == viewModel.Id);
                try {
                    db.Enterprises.Remove(ent);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "La empresa fue eliminada con éxito";
                } catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                return PartialView(viewModel);
            }
        }

        private void AddErrors(DbEntityValidationException exc) {
            foreach (
                var error in
                    exc.EntityValidationErrors.SelectMany(
                        validationErrors =>
                            validationErrors.ValidationErrors.Select(validationError => validationError.ErrorMessage)))
                ModelState.AddModelError("", error);
        }
    }
}