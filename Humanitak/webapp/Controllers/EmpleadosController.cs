using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using SmartAdminMvc.App_Helpers;
using SmartAdminMvc.Extensions;
using SmartAdminMvc.Helpers;
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
            var hasImss = !string.IsNullOrEmpty(viewModel.Ssn);
            var dailySalary = viewModel.CalculateSalary != null ? DeductionHelper.CalculateSalary(viewModel.DailySalary, viewModel.PaymentFrequency, hasImss) : viewModel.DailySalary;

            if (viewModel.Id == 0) {
                //new
                using (var db = new DataContext()) {
                    try {
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
                            PaymentFrequency = viewModel.PaymentFrequency,
                            Visible = true
                        };
                    
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
                emp.PaymentFrequency = viewModel.PaymentFrequency;
                emp.Visible = true;
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
                    emp.Visible = false;
                    //ent.Employees.Remove(emp);
                    //db.Employees.Remove(emp);
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

        public PartialViewResult _Bulk(int enterpriseId) {
            var viewModel = new EmployeeReferenceViewModel {
                Id = 0,
                Name = "",
                Processed = false,
                ProcessedMessage = "No hay datos para mostrar",
                Success = false,
                EnterpriseId = enterpriseId
            };
            return PartialView(viewModel);
        }

        [HttpPost]
        public PartialViewResult _Bulk(HttpPostedFileBase file, int enterpriseId) {
            var viewModel = new EmployeeReferenceViewModel {
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
            //var sheet = book.Worksheet(0);
            var total = sheet.Rows.Count();
            var columns = sheet.ColumnNames.ToList();
            using (var db = new DataContext()) {
                viewModel.Processed = true;
                foreach (var row in sheet.Rows) {
                    try {
                        var hasImss = !string.IsNullOrEmpty(row[columns[2]]);
                        var dailySalary = !string.IsNullOrEmpty(row[columns[7]]) && row[columns[7]].ToLower() == "si" ? 
                            DeductionHelper.CalculateSalary(double.Parse(row[columns[6]]), int.Parse(row[columns[35]]), hasImss) :
                            double.Parse(row[columns[6]]);
                        var parent = db.Enterprises.First(e => e.Id == enterpriseId);
                        Department department;
                        Position position;
                        Group group;
                        var payingName = row[columns[24]];
                        if(!db.Enterprises.Any(e => e.Name == payingName)) continue;
                        var paying = db.Enterprises.First(e => e.Name == payingName);
                        var secondaryName = row[columns[25]];
                        var secondary = db.Enterprises.FirstOrDefault(e => e.Name == secondaryName);
                        var departmentName = row[columns[22]];
                        if (!parent.Departments.Any(e => e.Name == departmentName)) {
                            department = new Department {
                                Name = departmentName,
                                Criteria = "Horarios Completos",
                                DoubleTimeHours = 0,
                                Overtime = false,
                                OvertimeThreshold = 0
                            };
                            db.Departments.AddOrUpdate(department);
                            parent.Departments.Add(department);
                            db.SaveChanges();
                        } else
                            department = parent.Departments.FirstOrDefault(e => e.Name == departmentName);
                        var positionName = row[columns[23]];
                        if (!parent.Positions.Any(e => e.Name != positionName)) {
                            position = new Position {
                                Name = positionName
                            };
                            db.Positions.AddOrUpdate(position);
                            parent.Positions.Add(position);
                            db.SaveChanges();
                        } else
                            position = parent.Positions.FirstOrDefault(e => e.Name == positionName);
                        var groupName = row[columns[26]];
                        if (!string.IsNullOrEmpty(groupName) && !parent.Groups.Any(e => e.Name == groupName)) {
                            group = new Group {
                                Name = groupName
                            };
                            db.Groups.AddOrUpdate(group);
                            parent.Groups.Add(group);
                            db.SaveChanges();
                        } else
                            group = parent.Groups.FirstOrDefault(e => e.Name == groupName);
                        var emp = new Employee {
                            Name = row[columns[0]],
                            LastName = row[columns[1]],
                            Ssn = row[columns[2]],
                            Curp = row[columns[3]],
                            Rfc = row[columns[4]],
                            Email = row[columns[27]],
                            Gender = row[columns[5]],
                            Department = department,
                            Position = position,
                            Group = group,
                            DailySalary = dailySalary,
                            StartDate = DateTime.ParseExact(row[columns[8]].Split(' ')[0], "d/M/yyyy", new CultureInfo("es-MX")),
                            Bank = row[columns[9]],
                            AccountNumber = row[columns[10]],
                            OffDays = row[columns[11]],
                            IsActive = true,
                            Address = row[columns[12]],
                            Area = row[columns[13]],
                            ZipCode = row[columns[14]],
                            City = row[columns[15]],
                            State = row[columns[16]],
                            Phone = row[columns[17]],
                            HasSocialSecurity = hasImss,
                            DoB = DateTime.ParseExact(row[columns[18]].Split(' ')[0], "d/M/yyyy", new CultureInfo("es-MX")),
                            SsRegistrationDate = DateTime.ParseExact(row[columns[28]].Split(' ')[0], "d/M/yyyy", new CultureInfo("es-MX")),
                            PlaceOfBirth = row[columns[19]],
                            IdNumber = row[columns[20]],
                            MaritalStatus = row[columns[21]],
                            PayingEnterprise = paying,
                            SecondaryEnterprise = secondary,
                            StartContractDate = DateTime.ParseExact(row[columns[29]].Split(' ')[0], "d/M/yyyy", new CultureInfo("es-MX")),
                            EndContractDate = string.IsNullOrEmpty(row[columns[30]]) ? (DateTime?)null : DateTime.ParseExact(row[columns[30]].Split(' ')[0], "d/M/yyyy", new CultureInfo("es-MX")),
                            PermanentContractDate = string.IsNullOrEmpty(row[columns[31]]) ? (DateTime?)null : DateTime.ParseExact(row[columns[31]].Split(' ')[0], "d/M/yyyy", new CultureInfo("es-MX")),
                            WorkState = row[columns[32]],
                            PatronalRegistryNo = row[columns[33]],
                            Regime = row[columns[34]],
                            PaymentFrequency = int.Parse(row[columns[35]]),
                            Visible = true,
                        };
                    
                        db.Employees.AddOrUpdate(emp);
                        db.SaveChanges();
                        parent.Employees.Add(emp);
                        db.SaveChanges();
                        inserted++;
                    }
                    catch (Exception e) {
                        viewModel.ProcessedMessage = e.Message;
                    }
                }
                viewModel.ProcessedMessage = $"{inserted} Empleados fueron insertados con éxito";
                viewModel.Success = total == inserted;
            }
            return PartialView(viewModel);
        }
    }
}