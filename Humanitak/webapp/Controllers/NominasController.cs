using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SmartAdminMvc.App_Helpers;
using SmartAdminMvc.Extensions;
using SmartAdminMvc.Helpers;
using SmartAdminMvc.Models;
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

        [HttpPost]
        [Authorize]
        public async Task<PartialViewResult> _Nueva(PayDayViewModel viewModel) {
            if (!ModelState.IsValid || viewModel == null) return _Nueva(0);
            viewModel.Processed = true;

            if (viewModel.Id != 0) return PartialView(viewModel);
            using (var db = new DataContext()) {
                var ent = db.Enterprises.First(e => e.Id == viewModel.EnterpriseId);
                var creator = await UserStore.FindByNameAsync(User.Identity.GetUserName());
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
                                nextPayDate = lastPayDate.Day == ent.Payday1End ? new DateTime(lastPayDate.Year, lastPayDate.Month, DateTime.DaysInMonth(lastPayDate.Year, lastPayDate.Month)) : new DateTime(lastPayDate.Year, lastPayDate.Month + 1, ent.Payday1End);
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
                            totalDays = nextPayDate.Subtract(lastPayDate).Days;
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
                var commPct =
                    Math.Round(!payDay.Enterprise.Clients.Any() ? 5 : payDay.Enterprise.Clients.First().Commission, 2);
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