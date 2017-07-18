using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DataAccess;
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
            Session["Empresa"] = null;
            var list = new List<EnterpriseViewModel>();
            using (var db = new DataContext()) {
                var user = (User)Session["User"];
                var enterprises = user.LinkedEnterprise == null
                    ? db.Enterprises.ToList()
                    : db.Clients.First(c => c.Id == user.LinkedEnterprise.Id).Enterprises;
                var ids = enterprises.Select(e => e.Id);
                enterprises.AddRange(db.Enterprises.Where(c => ids.Contains(c.ParentEnterprise.Id)));
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
                var emp = db.Enterprises.First(e => e.Id == id).ToEnterpriseReference();
                Session["Empresa"] = emp.Name;
                return View(emp);
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
                        Payday3Start = -1,
                        Payday4Start = -1,
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
                        Payday3Start = viewModel.Payday3Start,
                        Payday4Start = viewModel.Payday4Start,
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
                ent.Payday3Start = viewModel.Payday3Start;
                ent.Payday4Start = viewModel.Payday4Start;
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

            var sheet = DataTable.New.ReadExcel(path);
            var total = sheet.Rows.Count();
            var columns = sheet.ColumnNames.ToList();
            using (var db = new DataContext()) {
                viewModel.Processed = true;
                foreach (var row in sheet.Rows) {
                    var pay2End = 0;
                    int.TryParse(row[columns[4]], out pay2End);
                    Enterprise parentEnt = null;
                    if (row[columns[10]].ToLower() != "primaria") {
                        var name = row[columns[11]];
                        if (db.Enterprises.Any(e => e.Name == name))
                            parentEnt = db.Enterprises.First(e => e.Name == name);
                    }
                    var ent = new Enterprise {
                        City = row[columns[8]],
                        IsActive = true,
                        Name = row[columns[0]],
                        Operations = string.IsNullOrEmpty(row[columns[9]]) ? null : row[columns[9]],
                        Payday1Start = int.Parse(row[columns[1]]),
                        Payday1End = int.Parse(row[columns[2]]),
                        Payday2Start = int.Parse(row[columns[3]]),
                        Payday2End = pay2End,
                        Payday3Start = int.Parse(row[columns[5]]),
                        Payday4Start = int.Parse(row[columns[6]]),
                        State = row[columns[9]],
                        Vat = double.Parse(row[columns[8]]),
                        UsesPunchClock = !(string.IsNullOrEmpty(row[columns[7]]) || row[columns[7]].ToLower() == "no" || row[columns[7]].ToLower() == "0"),
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