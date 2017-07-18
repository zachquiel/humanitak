using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SmartAdminMvc.App_Helpers;
using SmartAdminMvc.Extensions;
using SmartAdminMvc.Helpers;
using SmartAdminMvc.Models;
using SmartAdminMvc.ServicioNomina;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Controllers {
    public class NominasController : Controller {
        private static readonly BaseUserStore UserStore = new BaseUserStore();
        // GET: Nominas
        public ActionResult Index(int id) {
            using (var db = new DataContext()) {
                return !db.Enterprises.Any()
                    ? View()
                    : View(db.Enterprises.First(e => e.Id == id).ToPaydayListViewModel());
            }
        }

        public PartialViewResult _Nueva(int id) {
            var viewModel = new PayDayViewModel {
                EnterpriseId = id,
                Processed = false,
                ProcessedMessage = "",
                Success = false
            };
            return PartialView(viewModel);
        }

        public PartialViewResult _Autorizar(long id) {

            using (var db = new DataContext()) {
                return PartialView(db.PayDays.First(p => p.Id == id).ToPayDayViewModel(0));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<PartialViewResult> _Autorizar(PayDayReference viewModel) {
            if (!ModelState.IsValid || viewModel == null) return _Autorizar(0);
            viewModel.Processed = true;
            var user = (User)Session["User"];
            using (var db = new DataContext()) {
                var pd = db.PayDays.First(e => e.Id == viewModel.Id);
                //    var ent = db.PayDays.First(e => e.Id == viewModel.Id).Enterprise;
                //    var emps = db.EmployeePayDays.Where(e => e.PayDay.Id == viewModel.Id).ToList();
                //    var states = db.Catalog.Where(c => c.Type == "State").ToList();
                //    var banks = db.Catalog.Where(c => c.Type == "Bank").ToList();
                //    var folio = 1;
                //    var totalEmps = emps.Count;
                //    var successful = 0;
                //    foreach (var emp in emps) {
                //        var employee = emp.Employee;
                //        var baseSalary = emp.Perceptions - emp.BreakDays - emp.DoublePay -
                //                         emp.TriplePay - emp.Holidays - emp.Overtime - emp.SundayPrime -
                //                         emp.VacationPrime - emp.Vacations;
                //        var multiplier = 30.4166;
                //        var factor = 1;
                //        if (employee.PaymentFrequency == 15) factor = 2;
                //        else if (employee.PaymentFrequency == 10) factor = 3;
                //        else if (employee.PaymentFrequency == 7) factor = 4;
                //        else if (employee.PaymentFrequency == 1) factor = 30;
                //        multiplier = multiplier / factor;

                //        var imssDeduction = 0d;
                //        if (employee.HasSocialSecurity) {
                //            imssDeduction = DeductionHelper.DeductImss(employee.DailySalary);
                //            imssDeduction +=
                //                DeductionHelper.DeductImss(employee.DailySalary, false) / 2;
                //            if (employee.PaymentFrequency == 1) imssDeduction = imssDeduction / 30;
                //            else if (employee.PaymentFrequency == 7) imssDeduction = imssDeduction / 4;
                //            else if (employee.PaymentFrequency == 10) imssDeduction = imssDeduction / 3;
                //            else if (employee.PaymentFrequency == 15) imssDeduction = imssDeduction / 2;
                //        }
                //        var isrDeduction = DeductionHelper.CalculateIsrDeduction((employee.DailySalary * multiplier) - imssDeduction, employee.PaymentFrequency);
                //        //return isrDeduction + imssDeduction;

                //        var emm = ObjetoCfdi.BuildEmmiterData("", employee.PatronalRegistryNo, ent.Client.FiscalInformation.Rfc);
                //        var basecf = ObjetoCfdi.BuildCfdiBase(DateTime.Now, emp.Perceptions, emp.Deductions, "", emp.Perceptions - emp.Deductions, 
                //            employee.ZipCode, ent.Name, folio.ToString()) +
                //            ObjetoCfdi.BuildEmployeeBase(employee.Rfc, employee.Name + employee.LastName, baseSalary);
                //        var percepcion = new List<string> {
                //            ObjetoCfdi.BuildPercepciones("001", "TRA", "Trabajo", baseSalary, 0,
                //                new[] {emp.NaturalDays}, new[] {"03"},
                //                new[] {emp.NaturalDays * 8}, new[] {100d})
                //        };
                //        if (emp.BreakDays > 0)
                //            percepcion.Add(ObjetoCfdi.BuildPercepciones("038", "TRA", "Descanso Trabajando", baseSalary, 0,
                //                new[] {emp.NaturalDays}, new[] {"03"},
                //                new[] {emp.NaturalDays * 8}, new[] {100d}));
                //        var deduction = new List<string>();
                //        if (imssDeduction > 0) 
                //            deduction.Add(ObjetoCfdi.BuildDeducciones("001","Seguridad social", "IMSS", imssDeduction));
                //        if(isrDeduction > 0)
                //            deduction.Add(ObjetoCfdi.BuildDeducciones("002", "ISR", "ISR", isrDeduction));


                //        var state = states.First(s => s.Name == employee.State);
                //        var bank = banks.FirstOrDefault(b => b.Name == employee.Bank);
                //        var ant = DateTime.Now.Subtract(employee.StartDate).TotalDays;
                //        var years = (int)(ant / 365);
                //        var days = (int)(ant - (years * 365));
                //        var period = employee.PaymentFrequency == 1 ? "01" :
                //            employee.PaymentFrequency == 7 ? "02" :
                //            employee.PaymentFrequency == 10 ? "03" :
                //            employee.PaymentFrequency == 15 ? "04" :
                //            employee.PaymentFrequency == 30 ? "05" : "99";
                //        var patrons = new[] {employee.PayingEnterprise.Client.FiscalInformation.Rfc};
                //        var comp = ObjetoCfdi.BuildPaymentComplement("O", emp.PayDate, emp.PayDate.AddDays(-emp.NaturalDays),
                //            emp.PayDate, emp.NaturalDays, emp.Perceptions - baseSalary,
                //            emm, employee.Id.ToString(), employee.Curp, employee.Regime == "Sueldos y Salarios" ? "02" : "11", 
                //            employee.Ssn, state.Key, "NO", employee.Department.Name, employee.AccountNumber, bank == null ? "000" : bank.Key,
                //            employee.StartDate, $"P{years}Y{days}D", employee.Position.Name, 
                //            employee.PermanentContractDate == null ? "03" : "01", "01", period, employee.DailySalary, "1", employee.DailySalary,
                //            patrons, new[] { 100d }, emp.Perceptions - emp.Deductions, 0, baseSalary, 0, 0, percepcion.ToArray(), "", "",
                //            0, imssDeduction +  isrDeduction, deduction.ToArray(), new[] { "" }, "");
                //        var service = new ComprobantesTimbradoSoapClient();
                //        //var response = service.TimbrarXMLconComplementos();
                //        var response = service.TimbrarTxT(new PeticionObtenerXML {
                //            usuario = "DEMO010203001",
                //            contrasena = "DEMO010203001a+",
                //            tipoCFDI = t_CFDI.RecibodeNomina12,
                //            rfcEmisor = ent.Client.FiscalInformation.Rfc,
                //            productivo = false,
                //            cadenaTXT = basecf + comp,
                //            cadenaComplemento = ""
                //        });
                //        folio++;
                //        if (!response.exito) {
                //            var errorPath = Path.Combine(Server.MapPath("~/RecibosNomina/errors"), emp.Id + ".txt");
                //            System.IO.File.WriteAllText(errorPath, employee.Id + Environment.NewLine + response.errorGeneral + Environment.NewLine + response.errorEspecifico);
                //            continue;
                //        }
                //        successful++;
                //        var pdfPath = Path.Combine(Server.MapPath("~/RecibosNomina/pdf"), response.uuid + ".pdf");
                //        var xmlPath = Path.Combine(Server.MapPath("~/RecibosNomina/xml"), response.uuid + ".xml");
                //        System.IO.File.WriteAllBytes(pdfPath, response.pdf);
                //        System.IO.File.WriteAllBytes(xmlPath, response.xmlTimbrado);
                //    }
                pd.Authorizer = user;
                pd.AuthotizationDate = DateTime.Today;
                db.SaveChanges();
                return PartialView(viewModel);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<PartialViewResult> _Nueva(PayDayViewModel viewModel) {
            if (!ModelState.IsValid || viewModel == null) return _Nueva(0);
            viewModel.Processed = true;
            var user = (User)Session["User"];
            if (viewModel.Id != 0) return PartialView(viewModel);
            using (var db = new DataContext()) {
                var ent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                var creator = user;//await UserStore.FindByNameAsync(User.Identity.GetUserName());
                var payStart = DateTime.ParseExact(viewModel.StartDate, "dd/MM/yyyy", new CultureInfo("es-MX"));
                var payDate = DateTime.ParseExact(viewModel.Date, "dd/MM/yyyy", new CultureInfo("es-MX"));
                var totalDays = payDate.Subtract(payStart).Days + 1;
                var pay = new PayDay {
                    Creator = creator,
                    Enterprise = ent,
                    PayDate = payDate,
                    PayStartDate = payStart,
                    AuthotizationDate = new DateTime(1970, 1, 1)
                };
                payStart = payStart.AddDays(-1);
                var payedEmployees = new List<EmployeePayDay>();
                foreach (var employee in ent.Employees.ToList()) {
                    EmployeePayDay lastPayDay = null;
                    var paydays = db.EmployeePayDays.ToList();
                    if (paydays.Any(epd => epd.Employee == employee)) {
                        lastPayDay = paydays.Last(epd => epd.Employee == employee);
                    }
                    var firstPay = new DateTime(payStart.Year, payStart.Month, ent.Payday1End);
                    var daysInMonth = DateTime.DaysInMonth(payStart.Year, payStart.Month);
                    var secondPay = new DateTime(payStart.Year, payStart.Month, ent.Payday2End == 0 || ent.Payday2End > daysInMonth ? daysInMonth : ent.Payday2End);
                    var nextPayDate = lastPayDay?.PayDate.AddMonths(1) ?? firstPay;
                    var multiplier = 30.4166;
                    var lastPayDate = lastPayDay?.PayDate ?? (payStart.Day <= ent.Payday1End ? secondPay : firstPay);
                    var factor = 1;
                    if (employee.PaymentFrequency == 15) {
                        factor = 2;
                        if (ent.Payday2End == 0) {
                            nextPayDate = lastPayDate.Day == ent.Payday1End ? new DateTime(lastPayDate.Year, lastPayDate.Month, DateTime.DaysInMonth(lastPayDate.Year, lastPayDate.Month)) : new DateTime(lastPayDate.Year, lastPayDate.Month + 1, ent.Payday1End);
                        } else
                            nextPayDate = lastPayDate.AddDays(15);
                    }
                    else if (employee.PaymentFrequency == 10) {
                        factor = 3;
                        nextPayDate = lastPayDay?.PayDate.AddDays(10) ?? firstPay;
                    }
                    else if (employee.PaymentFrequency == 7) {
                        factor = 4;
                        nextPayDate = lastPayDay?.PayDate.AddDays(7) ?? firstPay;
                    }
                    else if (employee.PaymentFrequency == 1) {
                        factor = 30;
                        nextPayDate = lastPayDay?.PayDate.AddDays(1) ?? firstPay;
                    }
                    multiplier = multiplier / factor;
                    var payment = employee.DailySalary * multiplier;

                    for (var day = 0; day <= Math.Abs(payStart.Subtract(payDate).Days); day++) {
                        if (employee.PaymentFrequency == 15) {
                            if (ent.Payday2End == 0) {
                                nextPayDate = lastPayDate.Day == ent.Payday1End ? new DateTime(lastPayDate.Year, lastPayDate.Month, DateTime.DaysInMonth(lastPayDate.Year, lastPayDate.Month)) : new DateTime(lastPayDate.Year, lastPayDate.Month, ent.Payday1End).AddMonths(1);
                            } else
                                nextPayDate = lastPayDate.AddDays(15);
                        }
                        else if (employee.PaymentFrequency == 10) {
                            nextPayDate = lastPayDate.AddDays(10);
                        }
                        else if (employee.PaymentFrequency == 7) {
                            nextPayDate = lastPayDate.AddDays(7);
                        }
                        else if (employee.PaymentFrequency == 1) {
                            nextPayDate = lastPayDate.AddDays(1);
                        }
                        var today = payStart.AddDays(day);
                        if (today.CompareTo(nextPayDate) == 0) {
                            totalDays = (int)nextPayDate.Subtract(lastPayDate).TotalDays;
                            var inc = db.Incidences.Where(i => i.Employee.Id == employee.Id && i.Date >= lastPayDate && i.Date <= nextPayDate).ToList();
                            var overTime = inc.Any(i => i.Type == "Horas Extra")
                                ? ((employee.DailySalary / 8) * 2) * inc.Where(i => i.Type == "Horas Extra").Sum(i => i.ExtraHours)
                                : 0;
                            var vacation = inc.Count(i => i.Type == "Vacaciones");
                            var vacationPrime = vacation * (employee.DailySalary * 0.25);
                            var doubleDay = inc.Count(i => i.Type == "Dia Doble") * (employee.DailySalary * 2);
                            var tripleDay = inc.Count(i => i.Type == "Dia Triple") * (employee.DailySalary * 3);
                            var deductions = DeductionHelper.CalculateDeductions(employee.DailySalary, employee.PaymentFrequency, employee.HasSocialSecurity);
                            var breakDay = inc.Count(i => i.Type == "Descanso Trabajando") * (employee.DailySalary);
                            payment = payment -
                                      ((inc.Count(i => i.Type == "Falta") + inc.Count(i => i.Type == "Dia Doble") +
                                        inc.Count(i => i.Type == "Dia Triple") +
                                        inc.Count(i => i.Type == "Descanso Trabajando") + inc.Count(i => i.Type == "Vacaciones")) *
                                       employee.DailySalary) + overTime + vacation + vacationPrime + tripleDay + doubleDay +
                                      breakDay;
                            var totalDaysforEmployee = totalDays - (!inc.Any() ? 0 : inc.Count(i => i.Type == "Falta"));
                            if(employee.StartDate > nextPayDate)
                                totalDaysforEmployee -= (int)employee.StartDate.Subtract(nextPayDate).TotalDays;
                            if(employee.StartDate > payDate) continue;
                            var payed = new EmployeePayDay
                            {
                                BreakDays = breakDay,
                                DailySalary = employee.DailySalary,
                                Deductions = deductions,
                                DoublePay = doubleDay,
                                Employee = employee,
                                Holidays = 0,
                                NaturalDays = totalDaysforEmployee,
                                Overtime = overTime,
                                PayDay = pay,
                                SundayPrime = 0,
                                TriplePay = tripleDay,
                                VacationPrime = vacationPrime,
                                Vacations = vacation,
                                Perceptions = payment,
                                PayDate = nextPayDate,
                            };
                            payedEmployees.Add(payed);
                            lastPayDate = nextPayDate;
                        }
                    }
                }
                try {
                    //db.PayDays.Add(pay);
                    db.Entry(pay.Creator).State = EntityState.Unchanged;
                    db.PayDays.Add(pay);
                    db.EmployeePayDays.AddRange(payedEmployees);
                    db.SaveChanges();
                    viewModel.Success = true;
                    viewModel.ProcessedMessage = "La nómina fue creada con éxito";
                }
                catch (Exception e) {
                    viewModel.Success = false;
                    viewModel.ProcessedMessage = e.Message;
                }
            }
            return PartialView(viewModel);
        }
        
        // GET: Detalle
        public ActionResult Detalle(int id) {
            var list = new List<PayDayDetailViewModel>();
            using (var db = new DataContext()) {
                foreach (var day in db.EmployeePayDays.Where(d => d.PayDay.Id == id).ToList())
                    list.Add(day.ToPayDayDetailViewModel());
            }
            return View(list);
        }

        // GET: Detalle
        public ActionResult DetalleFull(int id) {
            var list = new List<EmployeePayDayFullInfo>();
            using (var db = new DataContext()) {
                foreach (var day in db.EmployeePayDays.Where(d => d.PayDay.Id == id).ToList())
                    list.Add(day.ToPayDayFullDetailViewModel());
            }
            return View(list);
        }

        // GET: Totales
        public ActionResult Totales(int id) {
            using (var db = new DataContext())
                return View(db.EmployeePayDays.Where(d => d.PayDay.Id == id).ToPayDayDepartmentViewModel());
        }

        // GET: Resumen
        public ActionResult Resumen(int id) {
            using (var db = new DataContext()) {
                var payDay = db.PayDays.First(d => d.Id == id);
                var list = db.EmployeePayDays.Where(d => d.PayDay.Id == id);
                //var cli = db.Clients.FirstOrDefault(c => c.Enterprises.Contains(payDay.Enterprise));
                var commPct =
                    Math.Round(payDay.Enterprise.Client?.Commission ?? 2);
                var amt = Math.Round(list.Sum(d => d.Perceptions - d.Deductions), 2);
                var commAmt = amt*(commPct/100);
                var vat = (amt + commAmt)*(payDay.Enterprise.Vat/100);
                var final = (amt + commAmt) + vat;
                var ret = new PayDayFullInfo {
                    Name = "Total",
                    DailySalary = Math.Round(list.Sum(d => d.DailySalary), 2).ToString("C"),
                    NaturalDays = list.First().NaturalDays,
                    Income = Math.Round(list.Sum(d => d.Perceptions), 2).ToString("C"),
                    Perceptions = Math.Round(0d, 2).ToString("C"),
                    Deductions = Math.Round(list.Sum(d => d.Deductions), 2).ToString("C"),
                    FinalIncome = amt.ToString("C"),
                    Department = "Todos",
                    StartDate = payDay.PayStartDate.ToString("dd MMMM yyyy", new CultureInfo("es-MX")),
                    EndDate = payDay.PayDate.ToString("dd MMMM yyyy", new CultureInfo("es-MX")),
                    Status = payDay.Authorizer == null ? "En espera" : "Aprobada",
                    VatPct = payDay.Enterprise.Vat.ToString(CultureInfo.InvariantCulture) + "%",
                    Vat = vat.ToString("C"),
                    Commission = commAmt.ToString("C"),
                    CommissionPct = commPct.ToString(CultureInfo.InvariantCulture) + "%",
                    FinalAmount = Math.Round(final, 2).ToString("C")
                };
                return View(ret);
            }
        }

        // GET: Dias
        public ActionResult Dias() {
            return View();
        }
    }
}