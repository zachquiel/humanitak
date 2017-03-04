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
using LinqToExcel;

namespace SmartAdminMvc.Controllers {
    [Authorize]
    public class ClientesController : Controller {
        // GET: Catalogo
        public ActionResult Index() {
            var list = new List<ClientViewModel>();
            using (var db = new DataContext()) {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var client in db.Clients.ToList()) list.Add(client.ToClientViewModel());
            }
            return View(list);
        }

        // GET: Catalogo
        public ActionResult Cliente(int id) {
            using (var db = new DataContext()) {
                return View(db.Clients.First(e => e.Id == id).ToClientFullViewModel());
            }
        }

        public PartialViewResult _AddEnterprise(int id) {
             using (var db = new DataContext()) {
                var ents = db.Enterprises.ToList();
                return PartialView(db.Clients.First(e => e.Id == id).ToClientReferenceViewModel(ents));
            }
        }

        public PartialViewResult _RemoveEnterprise(int id, int enterpriseId) {
             using (var db = new DataContext()) {
                var vm = db.Clients.First(e => e.Id == id).ToClientFullViewModel();
                vm.EnterpriseId = enterpriseId;
                return PartialView(vm);
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _AddEnterprise(ClientReferenceViewModel viewModel) {
            using (var db = new DataContext()) {
                viewModel.Processed = true;
                var ent = db.Enterprises.FirstOrDefault(e => e.Id == viewModel.EnterpriseId);
                var cli = db.Clients.FirstOrDefault(c => c.Id == viewModel.Id);
                if (cli != null && cli.Enterprises == null) cli.Enterprises = new List<Enterprise>();
                if (cli == null) return PartialView(viewModel);
                cli.Enterprises.Add(ent);
                try {
                    db.Clients.AddOrUpdate(cli);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "La empresa se ligó con éxito";
                } catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                return PartialView(viewModel);
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _RemoveEnterprise(ClientViewModel viewModel) {
            using (var db = new DataContext()) {
                viewModel.Processed = true;
                var cli = db.Clients.FirstOrDefault(c => c.Id == viewModel.Id);
                if (cli != null) {
                    var ent = cli.Enterprises.FirstOrDefault(e => e.Id == viewModel.EnterpriseId);
                    cli.Enterprises.Remove(ent);
                }
                try {
                    db.Clients.AddOrUpdate(cli);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "La empresa se desligó con éxito";
                } catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                return PartialView(viewModel);
            }
        }


        public ActionResult GetResult() {
            var message = "Welcome";
            return new JsonResult {Data = message, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }

        public PartialViewResult _Upsert(int id) {
            if (id == 0) {
                using (var db = new DataContext()) {
                    return PartialView(new ClientInsertViewModel {
                        Clients =
                            db.Clients.Select(e => new ClientReference {
                                Id = e.Id,
                                Name = e.Name
                            }).ToList(),
                        Payday1End = -1,
                        Payday1Start = -1,
                        Payday2End = -1,
                        Payday2Start = -1,
                        State = "0"
                    });
                }
            }
            using (var db = new DataContext()) {
                var ents = db.Clients.Select(e => new ClientReference {
                    Id = e.Id,
                    Name = e.Name
                }).ToList();
                var vm = db.Clients.First(e => e.Id == id).ToClientInsertViewModel(ents);
                return PartialView(vm);
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _Upsert([Bind(Exclude = "logoImage, headerImage")] ClientInsertViewModel viewModel,
            HttpPostedFileBase logoImage, HttpPostedFileBase headerImage) {
            if (!ModelState.IsValid || viewModel == null) return _Upsert(0);

            byte[] logoImageData = null;
            byte[] headerImageData = null;
            if (Request.Files.Count <= 0 && viewModel.Id == 0) return _Upsert(0);
            var logoImage2 = Request.Files["logoImage"];
            if (logoImage2 == null && viewModel.Id == 0) return _Upsert(0);
            var headerImage2 = Request.Files["headerImage"];
            if (headerImage2 == null && viewModel.Id == 0) return _Upsert(0);
            if (viewModel.Id == 0) {
                //new
                viewModel.Processed = true;
                using (var binaryReader = new BinaryReader(logoImage2.InputStream))
                    logoImageData = binaryReader.ReadBytes(logoImage2.ContentLength);
                using (var binaryReader = new BinaryReader(headerImage2.InputStream))
                    headerImageData = binaryReader.ReadBytes(headerImage2.ContentLength);
                using (var db = new DataContext()) {
                    var fis = new FiscalInformation {
                        Area = viewModel.Area,
                        InnerNumeral = viewModel.InnerNumeral,
                        OuterNumeral = viewModel.OuterNumeral,
                        Rfc = viewModel.Rfc,
                        StreetAddress = viewModel.StreetAddress,
                        Town = viewModel.Town,
                        ZipCode = viewModel.ZipCode,
                    };

                    var ent = new Client {
                        City = viewModel.City,
                        Commission = viewModel.Commission,
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
                        LastPayday = new DateTime(1970, 1, 1)
                    };
                    try {
                        db.FiscalInformations.AddOrUpdate(fis);
                        db.SaveChanges();
                        ent.FiscalInformation = fis;
                        db.Clients.AddOrUpdate(ent);
                        db.SaveChanges();
                        viewModel.Success = true;
                        viewModel.ProcessedMessage = "El cliente fue insertado con éxito";
                    } catch (Exception e) {
                        viewModel.Success = false;
                        viewModel.ProcessedMessage = e.Message;
                    }
                    viewModel.Clients =
                        db.Clients.Select(e => new ClientReference {
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
                var fis = db.FiscalInformations.First(f => f.Id == viewModel.FiscalId);
                var ent = db.Clients.First(e => e.Id == viewModel.Id);
                ent.City = viewModel.City;
                ent.Commission = viewModel.Commission;
                if (headerImageData != null) {
                    ent.Header.Image = headerImageData;
                    db.EnterpriseImages.AddOrUpdate(ent.Header);
                }
                ent.IsActive = true;
                if (logoImageData != null) {
                    ent.Logo.Image = logoImageData;
                    db.EnterpriseImages.AddOrUpdate(ent.Logo);
                }

                fis.Area = viewModel.Area;
                fis.InnerNumeral = viewModel.InnerNumeral;
                fis.OuterNumeral = viewModel.OuterNumeral;
                fis.Rfc = viewModel.Rfc;
                fis.StreetAddress = viewModel.StreetAddress;
                fis.Town = viewModel.Town;
                fis.ZipCode = viewModel.ZipCode;
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
                try {
                    db.FiscalInformations.AddOrUpdate(fis);
                    db.Clients.AddOrUpdate(ent);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "El cliente fue modificado con éxito";
                } catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                viewModel.Clients =
                    db.Clients.Select(e => new ClientReference {
                        Id = e.Id,
                        Name = e.Name
                    }).ToList();
                return PartialView(viewModel);
            }
        }

        public PartialViewResult _Delete(int id) {
            using (var db = new DataContext()) {
                return PartialView(db.Clients.Where(e => e.Id == id).Select(e => new ClientReference {
                    Id = e.Id,
                    Name = e.Name
                }).First());
            }
        }

        [HttpPost]
        public PartialViewResult _Delete(ClientReference viewModel) {
            if (viewModel == null) return PartialView(viewModel);
            viewModel.Processed = true;
            using (var db = new DataContext()) {
                var ent = db.Clients.First(e => e.Id == viewModel.Id);
                try {
                    db.Clients.Remove(ent);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "El cliente fue eliminado con éxito";
                } catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                return PartialView(viewModel);
            }
        }

        public PartialViewResult _Bulk() {
            var viewModel = new ClientReference {
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
            var viewModel = new ClientReference {
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
                        var fis = new FiscalInformation {
                            Area = row[13].Value.ToString(),
                            InnerNumeral = row[11].Value.ToString(),
                            OuterNumeral = row[12].Value.ToString(),
                            Rfc = row[15].Value.ToString(),
                            StreetAddress = row[10].Value.ToString(),
                            Town = row[9].Value.ToString(),
                            ZipCode = row[14].Value.ToString(),
                        };
                        var pay2End = 0;
                        int.TryParse(row[4].Value.ToString(), out pay2End);
                        var ent = new Client {
                            City = row[9].Value.ToString(),
                            Commission = double.Parse(row[6].Value.ToString()),
                            IsActive = true,
                            Name = row[0].Value.ToString(),
                            Operations = null,
                            Payday1Start = int.Parse(row[1].Value.ToString()),
                            Payday1End = int.Parse(row[2].Value.ToString()),
                            Payday2Start = int.Parse(row[3].Value.ToString()),
                            Payday2End = pay2End,
                            State = row[8].Value.ToString(),
                            Vat = double.Parse(row[7].Value.ToString()),
                            UsesPunchClock = !(string.IsNullOrEmpty(row[7].Value.ToString()) || row[7].Value.ToString().ToLower() == "no" || row[5].Value.ToString().ToLower() == "0"),
                            LastPayday = new DateTime(1970, 1, 1)
                        };
                        try {
                            db.FiscalInformations.AddOrUpdate(fis);
                            db.SaveChanges();
                            ent.FiscalInformation = fis;
                            db.Clients.AddOrUpdate(ent);
                            db.SaveChanges();
                            inserted++;
                        }
                        catch (Exception e) {}
                    }
                    viewModel.ProcessedMessage = $"{inserted} Clientes fueron insertados con éxito";
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