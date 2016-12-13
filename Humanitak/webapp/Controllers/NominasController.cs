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
                return !db.Enterprises.Any() ? View() : View(db.Enterprises.First(e => e.Id == id).ToPaydayListViewModel());
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
                foreach (var employee in ent.Employees) {
                    var payment = employee.DailySalary*15.2083d;
                    var payed = new EmployeePayDay {
                        BreakDays = 0,
                        DailySalary = employee.DailySalary,
                        Deductions = CalculateDeductions(payment),
                        DoublePay = 0,
                        Employee = employee,
                        Holidays = 0,
                        NaturalDays = totalDays,
                        Overtime = 0,
                        PayDay = pay,
                        SundayPrime = 0,
                        TriplePay = 0,
                        VacationPrime = 0,
                        Vacations = 0,
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

        private double CalculateDeductions(double totalAmount) {
            var deduction = 0d;
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
            int reference;
            double rest;
            if (totalAmount >= 244.8) {
                rest = totalAmount - 0.01;
                reference = 0;
            }
            else if (totalAmount >= 2077.5) {
                rest = totalAmount - 244.81;
                reference = 1;
            }
            else if (totalAmount >= 3651) {
                rest = totalAmount - 2077.51;
                reference = 2;
            }
            else if (totalAmount >= 4244.1) {
                rest = totalAmount - 3651.01;
                reference = 3;
            }
            else if (totalAmount >= 5081.4) {
                rest = totalAmount - 4244.11;
                reference = 4;
            }
            else if (totalAmount >= 10248.45) {
                rest = totalAmount - 5081.41;
                reference = 5;
            }
            else if (totalAmount >= 16153.05) {
                rest = totalAmount - 10248.46;
                reference = 6;
            }
            else if (totalAmount >= 30838.8) {
                rest = totalAmount - 16153.06;
                reference = 7;
            }
            else if (totalAmount >= 41118.45) {
                rest = totalAmount - 30838.81;
                reference = 8;
            }
            else if (totalAmount >= 123355.2) {
                rest = totalAmount - 41118.46;
                reference = 9;
            }
            else {
                rest = totalAmount - 123355.21;
                reference = 10;
            }

            deduction += isrFixed[reference];
            deduction += rest*(isrExceed[reference]/100);
            return deduction;
        }

        // GET: Detalle
        public ActionResult Detalle(int id) {
            var list = new List<PayDayDetailViewModel>();
            using (var db = new DataContext()) {
                foreach (var day in db.EmployeePayDays.Where(d => d.PayDay.Id == id)) {
                    list.Add(day.ToPayDayDetailViewModel());
                }
            }
            return View(list);
        }

        // GET: Detalle
        public ActionResult DetalleFull(int id) {
            var list = new List<PayDayDetailViewModel>();
            using (var db = new DataContext()) {
                foreach (var day in db.EmployeePayDays.Where(d => d.PayDay.Id == id)) {
                    list.Add(day.ToPayDayDetailViewModel());
                }
            }
            return View(list);
        }

        // GET: Totales
        public ActionResult Totales(int id) {
            using (var db = new DataContext()) {
                return View(db.EmployeePayDays.Where(d => d.PayDay.Id == id).ToPayDayDepartmentViewModel());
            }
        }

        // GET: Resumen
        public ActionResult Resumen(int id) {
            using (var db = new DataContext()) {
                var payDay = db.PayDays.First(d => d.Id == id);
                var list = db.EmployeePayDays.Where(d => d.PayDay.Id == id);
                var commPct = Math.Round(!payDay.Enterprise.Clients.Any() ? 5 : payDay.Enterprise.Clients.First().Commission, 2);
                var amt = Math.Round(list.Sum(d => d.Perceptions - d.Deductions), 2);
                var commAmt = amt * (commPct / 100);
                var vat = (amt + commAmt) * (payDay.Enterprise.Vat / 100);
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
                    StartDate = payDay.PayStartDate.ToString("dd/MMMM/yyyy"),
                    EndDate = payDay.PayDate.ToString("dd/MMMM/yyyy"),
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