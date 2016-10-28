using System;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.App_Helpers;
using SmartAdminMvc.Extensions;
using SmartAdminMvc.Models;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Controllers {
    public class PuestoController : Controller {
        // GET: Puesto
        public ActionResult Index(int id) {
            using (var db = new DataContext())
                return View(db.Enterprises.First(e => e.Id == id).ToPositionListViewModel());
        }

        // GET: Nuevo
        public PartialViewResult _Upsert(int positionId, int enterpriseId) {
            if (positionId == 0) return PartialView(new PositionViewModel { EnterpriseId = enterpriseId });
            using (var db = new DataContext()) {
                var vm = db.Positions.First(e => e.Id == positionId).ToPositionViewModel(enterpriseId);
                return PartialView(vm);
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _Upsert(PositionViewModel viewModel) {
            if (!ModelState.IsValid || viewModel == null) return _Upsert(0, 0);
            viewModel.Processed = true;

            if (viewModel.Id == 0) { //new
                using (var db = new DataContext()) {
                    var parent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                    var pos = new Position {
                        Name = viewModel.Name
                    };
                    try {
                        db.Positions.AddOrUpdate(pos);
                        parent.Positions.Add(pos);
                        db.SaveChanges();
                        viewModel.Success = true;
                        viewModel.ProcessedMessage = "El puesto fue insertado con éxito";
                    } catch (Exception e) {
                        viewModel.Success = false;
                        viewModel.ProcessedMessage = e.Message;
                    }
                    return PartialView(viewModel);
                }
            }
            using (var db = new DataContext()) {
                var pos = db.Positions.First(e => e.Id == viewModel.Id);
                pos.Name = viewModel.Name;
                try {
                    db.Positions.AddOrUpdate(pos);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "El puesto fue modificado con éxito";
                } catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                return PartialView(viewModel);
            }
        }

        public PartialViewResult _Delete(int positionId, int enterpriseId) {
            using (var db = new DataContext()) {
                return PartialView(db.Positions.First(e => e.Id == positionId).ToPositionViewModel(enterpriseId));
            }
        }

        [HttpPost]
        public PartialViewResult _Delete(PositionViewModel viewModel) {
            if (viewModel == null) return PartialView(viewModel);
            viewModel.Processed = true;
            using (var db = new DataContext()) {
                var pos = db.Positions.First(e => e.Id == viewModel.Id);
                var ent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                try {
                    ent.Positions.Remove(pos);
                    db.Positions.Remove(pos);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "El puesto fue eliminado con éxito";
                } catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                return PartialView(viewModel);
            }
        }
    }
}