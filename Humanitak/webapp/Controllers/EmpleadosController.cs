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
                    SecondaryEnterpriseId = -1,
                    State = "0",
                    WorkState = "0",
                    Regime = "0",
                    CalculateSalary = "0",
                    HasSocialSecurity = "0"
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
            var dailySalary = viewModel.CalculateSalary != null ? CalculateSalary(viewModel.DailySalary) : viewModel.DailySalary;
            if (viewModel.Id == 0) {
                //new
                using (var db = new DataContext()) {
                    var parent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                    var paying = db.Enterprises.FirstOrDefault(e => e.Id == viewModel.PayingEnterpriseId);
                    var secondary = db.Enterprises.FirstOrDefault(e => e.Id == viewModel.SecondaryEnterpriseId);
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
                        DailySalary = dailySalary,
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
                        HasSocialSecurity = viewModel.HasSocialSecurity != null,
                        DoB = viewModel.DoB,
                        SsRegistrationDate = viewModel.SsRegistrationDate,
                        PlaceOfBirth = viewModel.PlaceOfBirth,
                        IdNumber = viewModel.IdNumber,
                        MaritalStatus = viewModel.MaritalStatus,
                        PayingEnterprise = paying,
                        SecondaryEnterprise = secondary,
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
                var secondary = db.Enterprises.FirstOrDefault(e => e.Id == viewModel.SecondaryEnterpriseId);
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
                emp.DailySalary = dailySalary;
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
                emp.HasSocialSecurity = viewModel.HasSocialSecurity != null;
                emp.DoB = viewModel.DoB;
                emp.SsRegistrationDate = viewModel.SsRegistrationDate;
                emp.PlaceOfBirth = viewModel.PlaceOfBirth;
                emp.IdNumber = viewModel.IdNumber;
                emp.MaritalStatus = viewModel.MaritalStatus;
                emp.PayingEnterprise = paying;
                emp.SecondaryEnterprise = secondary;
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

        public double CalculateSalary(double input) {
            
            var isr = CalculateIsr(input);
            var newInput = input;
            while (Math.Abs(isr-input) > 0) {
                newInput = (input - isr) + newInput;
                isr = CalculateIsr(newInput);

            }

            var imss = 0d;
            if (newInput >= 56081.025)
                imss = 1301.025;
            if (newInput <= 3652.71)
                imss = 50.91;
            if (imss <= 0) {
                imss = ((newInput * 100) / 97.625) - newInput;
            }
            return (newInput + imss) / 15.2083;
        }

        public double CalculateIsr(double input) {
            var chargeTable = new[] {
                new[] {0.01, 244.8, 0, 1.92},
                new[] {244.81, 2077.50, 4.65, 6.4},
                new[] {2077.51, 3651.00, 121.95, 10.88},
                new[] {3651.01, 4244.10, 293.25, 16},
                new[] {4244.11, 5081.40, 388.05, 17.92},
                new[] {5081.41, 10248.45, 538.2, 21.36},
                new[] {10248.46, 16153.05, 1641.75, 23.52},
                new[] {16153.06, 30838.80, 3030.60, 30},
                new[] {30838.81, 41118.45, 7436.25, 32},
                new[] {41118.46, 123355.20, 10725.75, 34},
                new[] {123355.21, double.MaxValue, 38686.35, 35}
            };
            var subsidyTable = new[] {
                new[] {0.01, 581.9, 133.9},
                new[] {581.91, 872.8, 133.8},
                new[] {872.81, 1142.40, 133.8},
                new[] {1142.41, 1163.80, 129.2},
                new[] {1163.81, 1462.50, 125.8},
                new[] {1462.51, 1551.70, 116.5},
                new[] {1551.71, 1755.10, 106.9},
                new[] {1755.11, 2047.60, 96.9},
                new[] {2047.61, 2340.10, 83.4},
                new[] {2340.11, 2428.40, 71.6},
                new[] {2428.41, double.MaxValue, 0}
            };
            var tableVal = chargeTable.First(c => c[0] <= input && c[1] >= input);
            var subsidyVal = subsidyTable.First(c => c[0] <= input && c[1] >= input);
            var minLim = tableVal[0];
            var baseVal = input - minLim;
            var ratio = tableVal[3];
            var result = baseVal * (ratio / 100);
            var fixedVal = tableVal[2];
            var isr = result + fixedVal;
            var subsidy = subsidyVal[2];
            var total = isr - subsidy;
            var grandTotal = input - total;

            Console.WriteLine("input: " + input + "\t isr:" + grandTotal);
            return grandTotal;
        }
    }
}