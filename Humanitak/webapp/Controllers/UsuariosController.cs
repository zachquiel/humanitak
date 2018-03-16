using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SmartAdminMvc.App_Helpers;
using SmartAdminMvc.Extensions;
using SmartAdminMvc.Models;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Controllers {
    public class UsuariosController : Controller {
        private readonly UserManager _manager = UserManager.Create();

        public ActionResult Index() {
            Session["Empresa"] = null;
            var list = new List<AccountRegistrationViewModel>();
            using (var db = new DataContext()) {
                var entRefs = db.Clients.ToList().Select(client => client.ToClientReference()).ToArray();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var user in db.Users) list.Add(user.ToAccountRegistrationViewModel(entRefs));
            }
            return View(list);
        }

        public PartialViewResult _Nuevo(string id) {
            using (var db = new DataContext()) {
                var entRefs = db.Clients.ToList().Select(client => client.ToClientReference()).ToArray();
                return PartialView(new AccountRegistrationViewModel {
                    EnterpriseCatalog = entRefs,
                    LinkedEnterprise = -1
                });
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> _NuevoMain(AccountRegistrationViewModel viewModel) {
            using (var db = new DataContext()) {
                var entRefs = db.Clients.ToList().Select(client => client.ToClientReference()).ToArray();
                if (!ModelState.IsValid || viewModel == null) return PartialView(new AccountRegistrationViewModel {
                    EnterpriseCatalog = entRefs
                });
                viewModel.EnterpriseCatalog = entRefs;
                viewModel.Processed = true;
                var ent = db.Clients.FirstOrDefault(e => e.Id == viewModel.LinkedEnterprise);
                var user = new User {
                    UserName = viewModel.FirstName + " " + viewModel.LastName,
                    Email = viewModel.Email,
                    LinkedEnterprise = ent,
                    CanIssuePayments =
                        viewModel.CanIssuePayments != null && viewModel.CanIssuePayments.ToLowerInvariant() == "on",
                    UserType = viewModel.UserType,
                    FirstName = viewModel.FirstName,
                    IsActive = true,
                    LastName = viewModel.LastName,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid().ToString(),
                    LastAccess = new DateTime(1970, 1, 1),
                    PasswordHash = viewModel.Password.Md5Hash()
                };
                try {
                    db.Users.AddOrUpdate(user);
                    db.SaveChanges();
                    //var result = await _manager.CreateAsync(user, viewModel.Password);

                    //if (!result.Succeeded) {
                    //    AddErrors(result);
                    //    foreach (var error in result.Errors)
                    //        viewModel.ProcessedMessage += error + Environment.NewLine;
                    //}
                    viewModel.Success = true;//result.Succeeded;
                    viewModel.ProcessedMessage = "El usuario fue insertado con éxito";
                }
                catch (DbEntityValidationException ex) {
                    viewModel.ProcessedMessage = ex.Message;
                    AddErrors(ex);
                }
                return PartialView(viewModel);
            }
        }


        public PartialViewResult _Editar(string id) {
            using (var db = new DataContext()) {
                var entRefs = db.Clients.ToList().Select(client => client.ToClientReference()).ToArray();
                return PartialView(db.Users.FirstOrDefault(u => u.Id == id).ToAccountUpdateViewModel(entRefs));
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> _EditarMain(AccountUpdateViewModel viewModel) {
            
            using (var db = new DataContext()) {
                var entRefs = db.Clients.ToList().Select(client => client.ToClientReference()).ToArray();
                viewModel.EnterpriseCatalog = entRefs;
                if (!ModelState.IsValid || viewModel == null) return PartialView(viewModel);
                var ent = db.Clients.FirstOrDefault(e => e.Id == viewModel.LinkedEnterprise);
                viewModel.Processed = true;
                var user = db.Users.First(u => u.Email == viewModel.Email);//await _manager.FindByEmailAsync(viewModel.Email);
                user.UserName = viewModel.FirstName + " " + viewModel.LastName;
                //user.BusinessType = viewModel.BusinessType;
                user.LinkedEnterprise = ent;
                user.CanIssuePayments = viewModel.CanIssuePayments != null &&
                                        viewModel.CanIssuePayments.ToLowerInvariant() == "on";
                user.UserType = viewModel.UserType;
                user.FirstName = viewModel.FirstName;
                user.IsActive = true;
                user.LastName = viewModel.LastName;
                try {
                    db.Entry(user.LinkedEnterprise).State = EntityState.Unchanged;
                    db.Users.AddOrUpdate(user);
                    db.SaveChanges();
                    //var result = await _manager.UpdateAsync(user);
                    //if (!result.Succeeded) {
                    //    AddErrors(result);
                    //    foreach (var error in result.Errors)
                    //        viewModel.ProcessedMessage += error + Environment.NewLine;
                    //}
                    //else {
                        viewModel.Success = true;
                        viewModel.ProcessedMessage = "El usuario fue editado con éxito";
                    //}
                }
                catch (DbEntityValidationException ex) {
                    viewModel.ProcessedMessage = ex.Message;
                    AddErrors(ex);
                }
                return PartialView(viewModel);
            }
        }

        public PartialViewResult _Eliminar(string id) {
            using (var db = new DataContext()) {
                var entRefs = db.Clients.ToList().Select(client => client.ToClientReference()).ToArray();
                return PartialView(db.Users.FirstOrDefault(u => u.Id == id).ToAccountUpdateViewModel(entRefs));
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> _Eliminar(AccountUpdateViewModel viewModel) {
            if (!ModelState.IsValid || viewModel == null) return PartialView(viewModel);
            viewModel.Processed = true;
            var user = await _manager.FindByEmailAsync(viewModel.Email);
            try {
                var result = await _manager.DeleteAsync(user);
                if (!result.Succeeded) {
                    AddErrors(result);
                    foreach (var error in result.Errors)
                        viewModel.ProcessedMessage += error + Environment.NewLine;
                }
                viewModel.Success = result.Succeeded;
                viewModel.ProcessedMessage = "El usuario fue eliminado con éxito";
            }
            catch (DbEntityValidationException ex) {
                viewModel.ProcessedMessage = ex.Message;
                AddErrors(ex);
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

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
        }
    }
}