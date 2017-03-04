using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinqToExcel;
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
                var enterprises = db.Enterprises.ToList();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var enterprise in enterprises)
                    list.Add(new EnterpriseViewModel {
                        Id = enterprise.Id,
                        Name = enterprise.Name,
                        Payday1Start = enterprise.Payday1Start,
                        Payday1End = enterprise.Payday1End,
                        Payday2Start = enterprise.Payday2Start,
                        Payday2End = enterprise.Payday2End,
                        LogoImage = enterprise.Logo?.Image,
                        HeaderImage = enterprise.Header?.Image,
                        UsesPunchClock = enterprise.UsesPunchClock ? "1" : "0",
                        //Commission = enterprise.Commission,
                        Vat = enterprise.Vat,
                        IsActive = enterprise.IsActive ? "1" : "0",
                        ParentEnterprise = enterprise.ParentEnterprise?.Id ?? 0,
                        Operations = enterprise.Operations,
                        City = enterprise.City,
                        LastPayday = enterprise.LastPayday,
                        State = enterprise.State
                    });
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

        public PartialViewResult _Bulk() {
            var viewModel = new EnterpriseReference
            {
                Id = 0,
                Name = "",
                Processed = false,
                ProcessedMessage = "No hay datos para mostrar",
                Success = false
            };
            return PartialView(viewModel);
        }

        [HttpPost]
        public PartialViewResult _Bulk(HttpPostedFileBase file) {
            var viewModel = new EnterpriseReference {
                Id = 0,
                Name = "",
                Processed = true,
                ProcessedMessage = "No hay datos para mostrar",
                Success = false
            };
            if (file == null || file.ContentLength <= 0) return PartialView(viewModel);
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath("~/Excel/"), fileName);
            file.SaveAs(path);
            var inserted = 0;
            using (var book = new ExcelQueryFactory(path)) {
                var sheet = book.Worksheet(0);
                var total = sheet.Count();
                using (var db = new DataContext()) {
                    viewModel.Processed = true;
                    foreach (var row in sheet) {
                        var pay2End = 0;
                        int.TryParse(row[4].Value.ToString(), out pay2End);
                        Enterprise parentEnt = null;
                        if (row[10].Value.ToString().ToLower() != "primaria") {
                            var name = row[11].Value.ToString();
                            if (db.Enterprises.Any(e => e.Name == name))
                                parentEnt = db.Enterprises.First(e => e.Name == name);
                        }
                        var ent = new Enterprise {
                            City = row[8].Value.ToString(),
                            IsActive = true,
                            Name = row[0].Value.ToString(),
                            Operations = string.IsNullOrEmpty(row[9].Value.ToString()) ? null : row[9].Value.ToString(),
                            Payday1Start = int.Parse(row[1].Value.ToString()),
                            Payday1End = int.Parse(row[2].Value.ToString()),
                            Payday2Start = int.Parse(row[3].Value.ToString()),
                            Payday2End = pay2End,
                            State = row[7].Value.ToString(),
                            Vat = double.Parse(row[6].Value.ToString()),
                            UsesPunchClock = !(string.IsNullOrEmpty(row[5].Value.ToString()) || row[5].Value.ToString().ToLower() == "no" || row[5].Value.ToString().ToLower() == "0"),
                            LastPayday = new DateTime(1970, 1, 1),
                            ParentEnterprise = parentEnt
                        };
                        try {
                            db.Enterprises.AddOrUpdate(ent);
                            db.SaveChanges();
                            inserted++;
                        }
                        catch (Exception e) {}
                    }
                    viewModel.ProcessedMessage = $"{inserted} Empresas fueron insertadas con éxito";
                    viewModel.Success = total == inserted;
                }
            }
            return PartialView(viewModel);
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