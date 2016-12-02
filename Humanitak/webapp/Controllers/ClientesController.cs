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
    public class ClientesController : Controller {
        // GET: Catalogo
        public ActionResult Index() {
            var list = new List<ClientViewModel>();
            using (var db = new DataContext()) {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var client in db.Clients) list.Add(client.ToClientViewModel());
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
                Enterprise parent = null;
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