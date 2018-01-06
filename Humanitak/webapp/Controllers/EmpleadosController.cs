using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using Novacode;
using SmartAdminMvc.App_Helpers;
using SmartAdminMvc.Extensions;
using SmartAdminMvc.Helpers;
using SmartAdminMvc.Models;
using SmartAdminMvc.ViewModels;
using Group = SmartAdminMvc.Models.Group;

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
                var banks = db.Catalog.Where(c => c.Type == "Bank").Select(c => c.Name).ToArray();
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
                    HasSocialSecurity = "0",
                    BankList = banks
                });
                var vm = db.Employees.First(e => e.Id == employeeId).ToEmployeeViewModel(enterpriseId);
                vm.Enterprises = mps;
                vm.Groups = gps;
                vm.Departments = dep;
                vm.Positions = pos;
                vm.BankList = banks;
                return PartialView(vm);
            }
        }

        public FileResult GenerateContract(long id) {
            using (var db = new DataContext()) {
                var emp = db.Employees.First(e => e.Id == id);
                var ent = emp.PayingEnterprise.Client;
                var fis = ent.FiscalInformation;
                var fullAddress = fis.StreetAddress + " " + fis.OuterNumeral +
                                 (string.IsNullOrEmpty(fis.InnerNumeral) ? "" : " Interior " + fis.InnerNumeral) +
                                 ", " + fis.Area + ", " + fis.Town + ", CP " + fis.ZipCode;
                var multiplier = 30.4166;
                var factor = 1;
                var frequency = "mensuales";
                if (emp.PaymentFrequency == 15) {
                    factor = 2;
                    frequency = "quincenales";
                }
                else if (emp.PaymentFrequency == 10) {
                    factor = 3;
                    frequency = "catorcenales";
                }
                else if (emp.PaymentFrequency == 7) {
                    factor = 4;
                    frequency = "semanales";
                }
                else if (emp.PaymentFrequency == 1) {
                    factor = 30;
                    frequency = "diarios";
                }
                multiplier = multiplier / factor;
                var salary = emp.DailySalary * multiplier;
                var deductions = DeductionHelper.CalculateDeductions(emp.DailySalary, emp.PaymentFrequency, emp.HasSocialSecurity);
                salary = salary - deductions;
                var fullSalary = salary.ToString("C") + " (" + salary.ToText() + ")";
                var fullPathOrig = Path.Combine(Server.MapPath("~/Templates/"), "Contrato.docx");
                var fullPath = Path.Combine(Server.MapPath("~/Templates/"), new Guid() + ".docx");
                System.IO.File.Copy(fullPathOrig, fullPath, true);
                using (var document = DocX.Load(fullPath)) {
                    document.ReplaceText("#NOMBREEMPRESA#", ent.Name);
                    document.ReplaceText("#DIRECCIONEMPRESA#", fullAddress);
                    document.ReplaceText("#REPRESENTANTELEGAL#", ent.LegalRepresentative);
                    document.ReplaceText("#NOMBREEMPLEADO#", emp.Name + " " + emp.LastName);
                    document.ReplaceText("#DIRECCIONEMPLEADO#", emp.Address);
                    document.ReplaceText("#CURPEMPLEADO#", emp.Curp);
                    document.ReplaceText("#NSSEMPLEADO#", emp.Ssn);
                    document.ReplaceText("#PUESTOEMPLEADO#", emp.Position.Name);
                    document.ReplaceText("#DIAINICIO#", "" + emp.StartDate.Day);
                    document.ReplaceText("#MESINICIO#", "" + emp.StartDate.Month.ToMonth());
                    document.ReplaceText("#ANYOINICIO#", "" + emp.StartDate.Year);
                    document.ReplaceText("#SALARIOEMPLEADO#", "" + fullSalary);
                    document.ReplaceText("#TEMPORALIDADPAGO#", "" + frequency);
                    document.ReplaceText("#DIACONTRATO#", "" + emp.StartContractDate.Day);
                    document.ReplaceText("#MESCONTRATO#", "" + emp.StartContractDate.Month.ToMonth().ToUpper());
                    document.ReplaceText("#ANYOCONTRATO#", "" + emp.StartContractDate.Year);
                    document.ReplaceText("#RFCEMPRESA#", "" + emp.Rfc);
                    document.Save();
                }

                var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read) {Position = 0};
                stream.Flush();
                return File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    "Contrato"+ emp.Id + ".docx");
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult _Upsert(EmployeeViewModel viewModel) {
            if (viewModel == null) return _Upsert(0, 0);
            viewModel.Processed = true;
            var hasImss = !string.IsNullOrEmpty(viewModel.Ssn);
            var dailySalary = viewModel.DailySalary;
            if (viewModel.ComplementSalary > 0)
                dailySalary = viewModel.CalculateSalary != null ? DeductionHelper.CalculateSalary(viewModel.DailySalary, viewModel.PaymentFrequency, hasImss) : viewModel.DailySalary;

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
                            ComplementSalary = viewModel.ComplementSalary,
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
                emp.ComplementSalary = viewModel.ComplementSalary;
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
                            DeductionHelper.CalculateSalary(double.Parse(row[columns[6]]), int.Parse(row[columns[36]]), hasImss) :
                            double.Parse(row[columns[6]]);
                        var complementSalary = double.Parse(row[columns[8]]);
                        var parent = db.Enterprises.First(e => e.Id == enterpriseId);
                        Department department;
                        Position position;
                        Group group;
                        var payingName = row[columns[25]];
                        if(!db.Enterprises.Any(e => e.Name == payingName)) continue;
                        var paying = db.Enterprises.First(e => e.Name == payingName);
                        var secondaryName = row[columns[26]];
                        var secondary = db.Enterprises.FirstOrDefault(e => e.Name == secondaryName);
                        var departmentName = row[columns[23]];
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
                        var positionName = row[columns[24]];
                        if (!parent.Positions.Any(e => e.Name != positionName)) {
                            position = new Position {
                                Name = positionName
                            };
                            db.Positions.AddOrUpdate(position);
                            parent.Positions.Add(position);
                            db.SaveChanges();
                        } else
                            position = parent.Positions.FirstOrDefault(e => e.Name == positionName);
                        var groupName = row[columns[27]];
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
                            Email = row[columns[28]],
                            Gender = row[columns[5]],
                            Department = department,
                            Position = position,
                            Group = group,
                            DailySalary = dailySalary,
                            ComplementSalary = complementSalary,
                            StartDate = DateTime.ParseExact(row[columns[9]].Split(' ')[0], "d/M/yyyy", new CultureInfo("es-MX")),
                            Bank = row[columns[10]],
                            AccountNumber = row[columns[11]],
                            OffDays = row[columns[12]],
                            IsActive = true,
                            Address = row[columns[13]],
                            Area = row[columns[14]],
                            ZipCode = row[columns[15]],
                            City = row[columns[16]],
                            State = row[columns[17]],
                            Phone = row[columns[18]],
                            HasSocialSecurity = hasImss,
                            DoB = DateTime.ParseExact(row[columns[19]].Split(' ')[0], "d/M/yyyy", new CultureInfo("es-MX")),
                            SsRegistrationDate = DateTime.ParseExact(row[columns[29]].Split(' ')[0], "d/M/yyyy", new CultureInfo("es-MX")),
                            PlaceOfBirth = row[columns[20]],
                            IdNumber = row[columns[21]],
                            MaritalStatus = row[columns[22]],
                            PayingEnterprise = paying,
                            SecondaryEnterprise = secondary,
                            StartContractDate = DateTime.ParseExact(row[columns[30]].Split(' ')[0], "d/M/yyyy", new CultureInfo("es-MX")),
                            EndContractDate = string.IsNullOrEmpty(row[columns[31]]) ? (DateTime?)null : DateTime.ParseExact(row[columns[31]].Split(' ')[0], "d/M/yyyy", new CultureInfo("es-MX")),
                            PermanentContractDate = string.IsNullOrEmpty(row[columns[32]]) ? (DateTime?)null : DateTime.ParseExact(row[columns[32]].Split(' ')[0], "d/M/yyyy", new CultureInfo("es-MX")),
                            WorkState = row[columns[33]],
                            PatronalRegistryNo = row[columns[34]],
                            Regime = row[columns[35]],
                            PaymentFrequency = int.Parse(row[columns[36]]),
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