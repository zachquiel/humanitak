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
                var payedEmployees = new List<EmployeePayDay>();
                foreach (var employee in ent.Employees.ToList()) {
                    var payment = employee.DailySalary*15.2083d;
                    var inc = db.Incidences.Where(i => i.Employee.Id == employee.Id && i.Date >= payStart && i.Date <= payDate);
                    var overTime = inc.Any(i => i.Type == "Horas Extra")
                        ? ((employee.DailySalary/8)*2)*inc.Where(i => i.Type == "Horas Extra").Sum(i => i.ExtraHours)
                        : 0;
                    var vacation = inc.Count(i => i.Type == "Vacaciones");
                    var vacationPrime = vacation*(employee.DailySalary*0.25);
                    var doubleDay = inc.Count(i => i.Type == "Dia Doble")*(employee.DailySalary*2);
                    var tripleDay = inc.Count(i => i.Type == "Dia Triple")*(employee.DailySalary*3);
                    var deductions = CalculateIsr(payment);
                    var breakDay = inc.Count(i => i.Type == "Descanso Trabajando")*(employee.DailySalary);
                    payment = payment -
                              ((inc.Count(i => i.Type == "Falta") + inc.Count(i => i.Type == "Dia Doble") +
                                inc.Count(i => i.Type == "Dia Triple") +
                                inc.Count(i => i.Type == "Descanso Trabajando") + inc.Count(i => i.Type == "Vacaciones"))*
                               employee.DailySalary) + overTime + vacation + vacationPrime + tripleDay + doubleDay +
                              breakDay;
                    var totalDaysforEmployee = totalDays - inc.Count(i => i.Type == "Falta");
                    var payed = new EmployeePayDay {
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
                        Perceptions = payment
                    };
                    payedEmployees.Add(payed);
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

        public double CalculateIsr(double input) {
            var maxImss = 1301.025;
            var minImss = 50.91;
            var imssDeduction = Math.Min(Math.Max((0.02375 * input), minImss), maxImss);
            input -= imssDeduction;
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
            return total + imssDeduction;
        }

        private double CalculateDeductions(double totalAmount) {
            var maxImss = 1301.025;
            var minImss = 50.91;
            var imssDeduction = Math.Min(Math.Max((0.02375 * totalAmount), minImss), maxImss);
            var deduction = 0d;
            var subsidyMax = new[] {
                581.90,
                872.80,
                1142.40,
                1163.80,
                1462.50,
                1551.70,
                1755.10,
                2047.60,
                2340.10,
                2428.40,
                0
            };
            var subsidy = new[] {
                200.85,
                200.70,
                200.70,
                193.80,
                188.70,
                174.75,
                160.35,
                145.35,
                125.10,
                107.40,
                0
            };
            var isrFixed = new[] {
                0,
                4.65,
                121.95,
                293.25,
                388.05,
                538.2,
                1641.75,
                3030.60,
                7436.25,
                10725.75,
                38686.35
            };
            var isrExceed = new[] {
                6.4,
                10.88,
                16,
                17.92,
                21.36,
                23.52,
                30,
                32,
                34,
                35
            };
            int reference, reference2 = 10;
            double rest;
            for (var i = 0; i < subsidyMax.Length; i++) {
                if (totalAmount > subsidyMax[i])
                    continue;
                reference2 = i;
            }
            if (totalAmount <= 244.8) {
                rest = totalAmount - 0.01;
                reference = 0;
            }
            else if (totalAmount <= 2077.5) {
                rest = totalAmount - 244.81;
                reference = 1;
            }
            else if (totalAmount <= 3651) {
                rest = totalAmount - 2077.51;
                reference = 2;
            }
            else if (totalAmount <= 4244.1) {
                rest = totalAmount - 3651.01;
                reference = 3;
            }
            else if (totalAmount <= 5081.4) {
                rest = totalAmount - 4244.11;
                reference = 4;
            }
            else if (totalAmount <= 10248.45) {
                rest = totalAmount - 5081.41;
                reference = 5;
            }
            else if (totalAmount <= 16153.05) {
                rest = totalAmount - 10248.46;
                reference = 6;
            }
            else if (totalAmount <= 30838.8) {
                rest = totalAmount - 16153.06;
                reference = 7;
            }
            else if (totalAmount <= 41118.45) {
                rest = totalAmount - 30838.81;
                reference = 8;
            }
            else if (totalAmount <= 123355.2) {
                rest = totalAmount - 41118.46;
                reference = 9;
            }
            else {
                rest = totalAmount - 123355.21;
                reference = 10;
            }

            deduction += isrFixed[reference];
            deduction += rest*(isrExceed[reference]/100);
            deduction -= subsidy[reference2];
            return deduction + imssDeduction;
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