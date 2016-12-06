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
    public class EmpleadosController : Controller {
        // GET: Empleados
        public ActionResult Index(int id) {
            using (var db = new DataContext())
                return View(db.Enterprises.First(e => e.Id == id).ToEmployeeListViewModel());
        }

        public ActionResult Nuevo() {
            return View();
        }

        // GET: Nuevo
        public PartialViewResult _Upsert(int employeeId, int enterpriseId) {
            using (var db = new DataContext()) {
                var emp =
                    db.Enterprises.Where(e => e.Id == enterpriseId || e.ParentEnterprise.Id == enterpriseId).ToList();
                var gps = new List<GroupReferenceViewModel>();
                var pos = new List<PositionViewModel>();
                var dep = new List<DepartmentReferenceViewModel>();
                var mps = new List<EnterpriseReference>();
                foreach (var ent in emp) {
                    gps.AddRange(ent.Groups.Select(g => new GroupReferenceViewModel {Id = g.Id, Name = g.Name}));
                    pos.AddRange(ent.Positions.Select(p => new PositionViewModel {Id = p.Id, Name = p.Name}));
                    dep.AddRange(ent.Departments.Select(d => new DepartmentReferenceViewModel {Id = d.Id, Name = d.Name}));
                    mps.Add(new EnterpriseReference {Id = ent.Id, Name = ent.Name});
                }
                if (employeeId == 0) return PartialView(new EmployeeViewModel {
                    EnterpriseId = enterpriseId,
                    Enterprises = mps,
                    Groups = gps,
                    Departments = dep,
                    Positions = pos,
                    DepartmentId = -1,
                    PositionId = -1,
                    GroupId = -1,
                    PayingEnterpriseId = -1,
                    State = "0",
                    WorkState = "0",
                    Regime = "0"
                });
                var vm = db.Employees.First(e => e.Id == employeeId).ToEmployeeViewModel(enterpriseId);
                vm.Enterprises = mps;
                vm.Groups = gps;
                vm.Departments = dep;
                vm.Positions = pos;
                return PartialView(vm);
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _Upsert(EmployeeViewModel viewModel) {
            if (!ModelState.IsValid || viewModel == null) return _Upsert(0, 0);
            viewModel.Processed = true;

            if (viewModel.Id == 0) {
                //new
                using (var db = new DataContext()) {
                    var parent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                    var paying = db.Enterprises.FirstOrDefault(e => e.Id == viewModel.PayingEnterpriseId);
                    var department = db.Departments.FirstOrDefault(e => e.Id == viewModel.DepartmentId);
                    var position = db.Positions.FirstOrDefault(e => e.Id == viewModel.PositionId);
                    var group = db.Groups.FirstOrDefault(e => e.Id == viewModel.GroupId);
                    var emp = new Employee {
                        Name = viewModel.FirstName,
                        LastName = viewModel.LastName,
                        Ssn = viewModel.Ssn,
                        Curp = viewModel.Curp,
                        Rfc = viewModel.Rfc,
                        Email = viewModel.Email,
                        Gender = viewModel.Gender,
                        Department = department,
                        Position = position,
                        Group = group,
                        DailySalary = viewModel.DailySalary,
                        StartDate = viewModel.StartDate,
                        Bank = viewModel.Bank,
                        AccountNumber = viewModel.AccountNumber,
                        OffDays = viewModel.OffDays,
                        IsActive = viewModel.IsActive,
                        Address = viewModel.Address,
                        Area = viewModel.Area,
                        ZipCode = viewModel.ZipCode,
                        City = viewModel.City,
                        State = viewModel.State,
                        Phone = viewModel.Phone,
                        HasSocialSecurity =
                            viewModel.HasSocialSecurity != null &&
                            viewModel.HasSocialSecurity.ToLowerInvariant() == "on",
                        DoB = viewModel.DoB,
                        SsRegistrationDate = viewModel.SsRegistrationDate,
                        PlaceOfBirth = viewModel.PlaceOfBirth,
                        IdNumber = viewModel.IdNumber,
                        MaritalStatus = viewModel.MaritalStatus,
                        PayingEnterprise = paying,
                        StartContractDate = viewModel.StartContractDate,
                        EndContractDate = viewModel.EndContractDate,
                        PermanentContractDate = viewModel.PermanentContractDate,
                        WorkState = viewModel.WorkState,
                        PatronalRegistryNo = viewModel.PatronalRegistryNo,
                        Regime = viewModel.Regime,
                    };
                    try {
                        db.Employees.AddOrUpdate(emp);
                        parent.Employees.Add(emp);
                        db.SaveChanges();
                        viewModel.Success = true;
                        viewModel.ProcessedMessage = "El empleado fue insertado con éxito";
                    }
                    catch (Exception e) {
                        viewModel.Success = false;
                        viewModel.ProcessedMessage = e.Message;
                    }
                    return PartialView(viewModel);
                }
            }
            using (var db = new DataContext()) {
                var paying = db.Enterprises.FirstOrDefault(e => e.Id == viewModel.PayingEnterpriseId);
                var department = db.Departments.FirstOrDefault(e => e.Id == viewModel.DepartmentId);
                var position = db.Positions.FirstOrDefault(e => e.Id == viewModel.PositionId);
                var group = db.Groups.FirstOrDefault(e => e.Id == viewModel.GroupId);
                var emp = db.Employees.First(e => e.Id == viewModel.Id);
                emp.Name = viewModel.FirstName;
                emp.LastName = viewModel.LastName;
                emp.Ssn = viewModel.Ssn;
                emp.Curp = viewModel.Curp;
                emp.Rfc = viewModel.Rfc;
                emp.Email = viewModel.Email;
                emp.Gender = viewModel.Gender;
                emp.Department = department;
                emp.Position = position;
                emp.Group = group;
                emp.DailySalary = viewModel.DailySalary;
                emp.StartDate = viewModel.StartDate;
                emp.Bank = viewModel.Bank;
                emp.AccountNumber = viewModel.AccountNumber;
                emp.OffDays = viewModel.OffDays;
                emp.IsActive = viewModel.IsActive;
                emp.Address = viewModel.Address;
                emp.Area = viewModel.Area;
                emp.ZipCode = viewModel.ZipCode;
                emp.City = viewModel.City;
                emp.State = viewModel.State;
                emp.Phone = viewModel.Phone;
                emp.HasSocialSecurity =
                            viewModel.HasSocialSecurity != null &&
                            viewModel.HasSocialSecurity.ToLowerInvariant() == "on";
                emp.DoB = viewModel.DoB;
                emp.SsRegistrationDate = viewModel.SsRegistrationDate;
                emp.PlaceOfBirth = viewModel.PlaceOfBirth;
                emp.IdNumber = viewModel.IdNumber;
                emp.MaritalStatus = viewModel.MaritalStatus;
                emp.PayingEnterprise = paying;
                emp.StartContractDate = viewModel.StartContractDate;
                emp.EndContractDate = viewModel.EndContractDate;
                emp.PermanentContractDate = viewModel.PermanentContractDate;
                emp.WorkState = viewModel.WorkState;
                emp.PatronalRegistryNo = viewModel.PatronalRegistryNo;
                emp.Regime = viewModel.Regime;
                try {
                    db.Employees.AddOrUpdate(emp);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "El empleado fue modificado con éxito";
                }
                catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
                return PartialView(viewModel);
            }
        }

        public PartialViewResult _Delete(int employeeId, int enterpriseId) {
            using (var db = new DataContext()) {
                return
                    PartialView(db.Employees.First(e => e.Id == employeeId).ToEmployeeReferenceViewModel(enterpriseId));
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _Delete(EmployeeReferenceViewModel viewModel) {
            if (viewModel == null) return PartialView(viewModel);
            viewModel.Processed = true;
            using (var db = new DataContext()) {
                var emp = db.Employees.First(e => e.Id == viewModel.Id);
                var ent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                try {
                    ent.Employees.Remove(emp);
                    db.Employees.Remove(emp);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "El empleado fue eliminado con éxito";
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