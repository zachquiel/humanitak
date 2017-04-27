using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.App_Helpers;
using SmartAdminMvc.Models;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Controllers
{
    public class CvUploadController : Controller
    {
        // GET: CvUpload
        public ActionResult Index() {
            return View(new CvViewModel {
                ProcessedMessage = "",
                Success = false,
                Processed = false,
                Name = "",
                Email_cv = "",
                FileName = "",
                Date = ""
            });
        }

        [HttpPost]
        public ActionResult Index(CvViewModel viewModel) {
            viewModel.Processed = true;
            try {
                if (Request.Files.Count <= 0) {
                    viewModel.ProcessedMessage = "Debes seleccionar un archivo";
                    return View(viewModel);
                }
                var file = Request.Files[0];
                if (file == null || file.ContentLength <= 0) {
                    viewModel.ProcessedMessage = "Debes seleccionar un archivo";
                    return View(viewModel);
                }
                if(string.IsNullOrEmpty(viewModel.Name) || string.IsNullOrEmpty(viewModel.Email_cv)) {
                    viewModel.ProcessedMessage = "Debes llenar todos los campos";
                    return View(viewModel);
                }

                var fileName = Path.GetFileName(file.FileName);
                var date = DateTime.Today.ToString("dd/MM/yyyy");
                using (var db = new DataContext()) {
                    db.ResumeInfo.Add(new ResumeInfo {
                        Name = viewModel.Name,
                        Email = viewModel.Email_cv,
                        FileName = fileName,
                        Date = date
                    });
                    var path = Path.Combine(Server.MapPath("~/Cvs/"), fileName);
                    file.SaveAs(path);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "¡El archivo se envió con éxito!\n Pronto te contactaremos";
                }
            } catch (Exception e) {
                viewModel.ProcessedMessage = e.Message;
                viewModel.Success = false;
            }
            return View(viewModel);
        }
    }
}