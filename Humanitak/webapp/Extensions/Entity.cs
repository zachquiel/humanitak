using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SmartAdminMvc.Models;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Extensions {
    public static partial class Extensions {
        public static EnterpriseInfoViewModel ToEnterpriseInfoViewModel(this Enterprise enterprise) {
            var employeeCount = enterprise.Employees.Count(e => e.Visible);
            return new EnterpriseInfoViewModel {
                Id = enterprise.Id,
                Name = enterprise.Name,
                Departments = enterprise.Departments.Select(d => d.ToDepartmentViewModel(enterprise.Id)).ToList(),
                Perceptions = enterprise.Perceptions.Select(p => p.ToPerceptionViewModel(enterprise.Id)).ToList(),
                Positions = enterprise.Positions.Select(p => p.ToPositionViewModel(enterprise.Id)).ToList(),
                Employees = enterprise.Employees.Where(e => e.Visible).Select(e => e.ToEmployeeViewModel(enterprise.Id)).ToList(),
                PayDays = enterprise.PayDays.Select(e => e.ToPayDayViewModel(employeeCount)).ToList()
            };
        }

        public static EnterpriseInfoViewModel ToEmployeeListViewModel(this Enterprise enterprise) {
            return new EnterpriseInfoViewModel {
                Id = enterprise.Id,
                Name = enterprise.Name,
                Employees = enterprise.Employees.Where(e => e.Visible).Select(e => e.ToEmployeeViewModel(enterprise.Id)).ToList()
            };
        }

        public static EnterpriseIncidenceInfoViewModel ToIncidenceInfoViewModel(this Enterprise enterprise, List<Incidence> incidences) {
            return new EnterpriseIncidenceInfoViewModel {
                Id = enterprise.Id,
                Name = enterprise.Name,
                Employees = enterprise.Employees.Where(e => e.Visible).Select(e => e.ToEmployeeViewModel(enterprise.Id)).ToList(),
                Incidences = incidences.Select(i => i.ToIncidenceViewModel(enterprise.Id)).ToList(),

            };
        }

        public static IncidenceViewModel ToIncidenceViewModel(this Incidence incidence, long enterpriseId) {
            return new IncidenceViewModel {
                Id = incidence.Id,
                Date = incidence.Date,
                Employee = incidence.Employee.ToEmployeeReferenceViewModel(enterpriseId),
                Enterprise = incidence.Enterprise.ToEnterpriseReference(),
                ExtraHours = incidence.ExtraHours.ToString(),
                Type = incidence.Type,
                EnterpriseId = enterpriseId,
                EmployeeName = incidence.Employee.Name + " " + incidence.Employee.LastName,
                StringDate = incidence.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                TypeIndex = incidence.Type == "Falta" ? 1 : incidence.Type == "Vacaciones" ? 2 : 
                incidence.Type == "Descanso Trabajando" ? 3 : incidence.Type == "Dia Festivo" ? 4 : incidence.Type == "Dia Doble" ?
                5 : incidence.Type == "Dia Triple" ? 6 : 7
            };
        }

        public static EmployeeReferenceViewModel ToEmployeeReferenceViewModel(this Employee employee, long enterpriseId) {
            return new EmployeeReferenceViewModel {
                Id = employee.Id,
                Name = employee.Name + " " + employee.LastName,
                EnterpriseId = enterpriseId
            };
        }

        public static EmployeeViewModel ToEmployeeViewModel(this Employee employee, long enterpriseId) {
            return new EmployeeViewModel {
                Id = employee.Id,
                Name = employee.Name + " " + employee.LastName,
                FirstName = employee.Name,
                LastName = employee.LastName,
                Ssn = employee.Ssn,
                Curp = employee.Curp,
                Rfc = employee.Rfc,
                Email = employee.Email,
                Gender = employee.Gender,
                DepartmentId = employee.Department?.Id ?? -1,
                DepartmentName = employee.Department?.Name ?? "",
                GroupId = employee.Group?.Id ?? -1,
                GroupName = employee.Group?.Name ?? "",
                PositionId = employee.Position?.Id ?? -1,
                PositionName = employee.Position?.Name ?? "",
                DailySalary = employee.DailySalary,
                StartDate = employee.StartDate,
                Bank = employee.Bank,
                AccountNumber = employee.AccountNumber,
                OffDays = employee.OffDays,
                IsActive = employee.IsActive,
                Address = employee.Address,
                Area = employee.Area,
                ZipCode = employee.ZipCode,
                City = employee.City,
                State = employee.State,
                Phone = employee.Phone,
                HasSocialSecurity = employee.HasSocialSecurity ? "1" : "0",
                DoB = employee.DoB,
                SsRegistrationDate = employee.SsRegistrationDate,
                PlaceOfBirth = employee.PlaceOfBirth,
                IdNumber = employee.IdNumber,
                MaritalStatus = employee.MaritalStatus,
                EnterpriseId = enterpriseId,
                PayingEnterpriseId = employee.PayingEnterprise?.Id ?? -1,
                PayingEnterpriseName = employee.PayingEnterprise?.Name ?? "",
                StartContractDate = employee.StartContractDate,
                EndContractDate = employee.EndContractDate,
                PermanentContractDate = employee.PermanentContractDate,
                WorkState = employee.WorkState,
                PatronalRegistryNo = employee.PatronalRegistryNo,
                CalculateSalary = "0",
                Regime = employee.Regime,
                PaymentFrequency = employee.PaymentFrequency,
            };
        }

        public static EnterpriseInfoViewModel ToDepartmentListViewModel(this Enterprise enterprise) {
            return new EnterpriseInfoViewModel {
                Id = enterprise.Id,
                Name = enterprise.Name,
                Departments = enterprise.Departments.Select(d => d.ToDepartmentViewModel(enterprise.Id)).ToList(),
                SpecialPerceptions =
                    enterprise.SpecialPerceptions.Select(p => p.ToSpecialPerceptionViewModel(enterprise.Id)).ToList()
            };
        }

        public static DepartmentViewModel ToDepartmentViewModel(this Department department, long enterpriseId) {
            return new DepartmentViewModel {
                Id = department.Id,
                Name = department.Name,
                Criteria = department.Criteria,
                DoubleTimeHours = department.DoubleTimeHours,
                Overtime = department.Overtime,
                OvertimeThreshold = department.OvertimeThreshold,
                EnterpriseId = enterpriseId,
                EmployeeCount = department.Employees.Count
            };
        }

        public static EnterpriseInfoViewModel ToPerceptionListViewModel(this Enterprise enterprise) {
            return new EnterpriseInfoViewModel {
                Id = enterprise.Id,
                Name = enterprise.Name,
                Perceptions = enterprise.Perceptions.Select(p => p.ToPerceptionViewModel(enterprise.Id)).ToList(),
                SpecialPerceptions =
                    enterprise.SpecialPerceptions.Select(p => p.ToSpecialPerceptionViewModel(enterprise.Id)).ToList()
            };
        }

        public static PerceptionReferenceViewModel ToPerceptionReferenceViewModel(this Perception perception) {
            return new PerceptionReferenceViewModel {
                Id = perception.Id,
                Description = perception.Description
            };
        }

        public static GroupReferenceViewModel ToGroupReferenceViewModel(this Group group) {
            return new GroupReferenceViewModel {
                Id = group.Id,
                Name = group.Name
            };
        }

        public static SpecialPerceptionInsertViewModel ToSpecialPerceptionInsertViewModel(
            this SpecialPerception specialPerception, List<GroupReferenceViewModel> groups,
            List<PerceptionReferenceViewModel> perceptions, long enterpriseId) {
            var vm = (SpecialPerceptionInsertViewModel) specialPerception.ToSpecialPerceptionViewModel(enterpriseId);
            vm.Groups = groups;
            vm.Perceptions = perceptions;
            return vm;
        }

        public static SpecialPerceptionInsertViewModel ToSpecialPerceptionInsertViewModel(
            this SpecialPerception specialPerception, List<DepartmentReferenceViewModel> departments,
            List<PerceptionReferenceViewModel> perceptions, long enterpriseId) {
            var vm = (SpecialPerceptionInsertViewModel) specialPerception.ToSpecialPerceptionViewModel(enterpriseId);
            vm.Perceptions = perceptions;
            vm.Departments = departments;
            return vm;
        }

        public static SpecialPerceptionViewModel ToSpecialPerceptionViewModel(this SpecialPerception specialPerception,
            long enterpriseId) {
            return new SpecialPerceptionViewModel {
                Id = specialPerception.Id,
                KeyName = specialPerception.Perception.KeyName,
                Description = specialPerception.Perception.Description,
                Formula = specialPerception.Perception.Formula,
                Amount = specialPerception.Amount,
                Repeat = specialPerception.Repeat,
                EnterpriseId = enterpriseId,
                PerceptionId = specialPerception.Perception.Id,
                GroupId = specialPerception.Group?.Id ?? 0,
                GroupName = specialPerception.Group?.Name ?? "",
                DepartmentId = specialPerception.Department?.Id ?? 0,
                DepartmentName = specialPerception.Department?.Name ?? ""
            };
        }

        public static PerceptionViewModel ToPerceptionViewModel(this Perception perception, long enterpriseId) {
            return new PerceptionViewModel {
                Id = perception.Id,
                KeyName = perception.KeyName,
                Description = perception.Description,
                Formula = perception.Formula,
                EnterpriseId = enterpriseId
            };
        }

        public static EnterpriseInfoViewModel ToPositionListViewModel(this Enterprise enterprise) {
            return new EnterpriseInfoViewModel {
                Id = enterprise.Id,
                Name = enterprise.Name,
                Positions = enterprise.Positions.Select(p => p.ToPositionViewModel(enterprise.Id)).ToList()
            };
        }

        public static PositionViewModel ToPositionViewModel(this Position position, long enterpriseId) {
            return new PositionViewModel {
                Id = position.Id,
                Name = position.Name,
                EnterpriseId = enterpriseId
            };
        }

        public static EnterpriseReference ToEnterpriseReference(this Enterprise enterprise) {
            return new EnterpriseReference {
                Id = enterprise.Id,
                Name = enterprise.Name
            };
        }

        public static ClientReference ToClientReference(this Client client) {
            return new ClientReference {
                Id = client.Id,
                Name = client.Name
            };
        }

        public static EnterpriseViewModel ToEnterpriseViewModel(this Enterprise enterprise) {
            return new EnterpriseViewModel {
                Id = enterprise.Id,
                Name = enterprise.Name,
                Payday1Start = enterprise.Payday1Start,
                Payday1End = enterprise.Payday1End,
                Payday2Start = enterprise.Payday2Start,
                Payday2End = enterprise.Payday2End,
                LogoImage = enterprise.Logo?.Image,
                HeaderImage = enterprise.Header?.Image,
                UsesPunchClock = enterprise.UsesPunchClock ? "1" : "0",
                //Commission = enterprise.Commission,
                Vat = enterprise.Vat,
                IsActive = enterprise.IsActive ? "1" : "0",
                //ParentEnterprise = enterprise.ParentEnterprise?.Id ?? 0,
                Operations = enterprise.Operations,
                City = enterprise.City,
                LastPayday = enterprise.LastPayday,
                State = enterprise.State
            };
        }

        public static ClientViewModel ToClientViewModel(this Client client) {
            return new ClientViewModel {
                Id = client.Id,
                Name = client.Name,
                Payday1Start = client.Payday1Start,
                Payday1End = client.Payday1End,
                Payday2Start = client.Payday2Start,
                Payday2End = client.Payday2End,
                LogoImage = client.Logo?.Image,
                HeaderImage = client.Header?.Image,
                UsesPunchClock = client.UsesPunchClock ? "1" : "0",
                Commission = client.Commission,
                Vat = client.Vat,
                IsActive = client.IsActive ? "1" : "0",
                Operations = client.Operations,
                City = client.City,
                LastPayday = client.LastPayday,
                State = client.State
            };
        }

        public static ClientReferenceViewModel ToClientReferenceViewModel(this Client client,
            List<Enterprise> enterprises) {
            var ret = new ClientReferenceViewModel {
                Id = client.Id,
                Name = client.Name,
                Enterprises = new List<EnterpriseViewModel>()
            };
            foreach (
                var enterprise in enterprises.Where(enterprise => client.Enterprises.All(e => e.Id != enterprise.Id)))
                ret.Enterprises.Add(enterprise.ToEnterpriseViewModel());
            return ret;
        }

        public static ClientViewModel ToClientFullViewModel(this Client client) {
            var ret = new ClientViewModel {
                Id = client.Id,
                Name = client.Name,
                Payday1Start = client.Payday1Start,
                Payday1End = client.Payday1End,
                Payday2Start = client.Payday2Start,
                Payday2End = client.Payday2End,
                LogoImage = client.Logo?.Image,
                HeaderImage = client.Header?.Image,
                UsesPunchClock = client.UsesPunchClock ? "1" : "0",
                Commission = client.Commission,
                Vat = client.Vat,
                IsActive = client.IsActive ? "1" : "0",
                Operations = client.Operations,
                City = client.City,
                LastPayday = client.LastPayday,
                State = client.State,
                Enterprises = new List<EnterpriseViewModel>()
            };
            foreach (var enterprise in client.Enterprises) ret.Enterprises.Add(enterprise.ToEnterpriseViewModel());
            return ret;
        }

        public static EnterpriseInsertViewModel ToEnterpriseInsertViewModel(this Enterprise enterprise,
            List<EnterpriseReference> references) {
            return new EnterpriseInsertViewModel {
                Id = enterprise.Id,
                Name = enterprise.Name,
                Payday1Start = enterprise.Payday1Start,
                Payday1End = enterprise.Payday1End,
                Payday2Start = enterprise.Payday2Start,
                Payday2End = enterprise.Payday2End,
                Payday3Start = enterprise.Payday3Start,
                Payday4Start = enterprise.Payday4Start,
                LogoImage = enterprise.Logo?.Image,
                HeaderImage = enterprise.Header?.Image,
                UsesPunchClock = enterprise.UsesPunchClock ? "1" : "0",
                //Commission = enterprise.Commission,
                Vat = enterprise.Vat,
                IsActive = enterprise.IsActive ? "1" : "0",
                ParentEnterprise = enterprise.ParentEnterprise?.Id ?? 0,
                Operations = enterprise.Operations,
                City = enterprise.City,
                LastPayday = enterprise.LastPayday,
                State = enterprise.State,
                Enterprises = references
            };
        }

        public static ClientInsertViewModel ToClientInsertViewModel(this Client client,
            List<ClientReference> references) {
            return new ClientInsertViewModel {
                Id = client.Id,
                Name = client.Name,
                Payday1Start = client.Payday1Start,
                Payday1End = client.Payday1End,
                Payday2Start = client.Payday2Start,
                Payday2End = client.Payday2End,
                LogoImage = client.Logo?.Image,
                HeaderImage = client.Header?.Image,
                UsesPunchClock = client.UsesPunchClock ? "1" : "0",
                Commission = client.Commission,
                Vat = client.Vat,
                IsActive = client.IsActive ? "1" : "0",
                Operations = client.Operations,
                City = client.City,
                LastPayday = client.LastPayday,
                State = client.State,
                Clients = references,
                FiscalId = client.FiscalInformation.Id,
                Area = client.FiscalInformation.Area,
                InnerNumeral = client.FiscalInformation.InnerNumeral,
                OuterNumeral = client.FiscalInformation.OuterNumeral,
                Rfc = client.FiscalInformation.Rfc,
                StreetAddress = client.FiscalInformation.StreetAddress,
                Town = client.FiscalInformation.Town,
                ZipCode = client.FiscalInformation.ZipCode
            };
        }

        public static AccountRegistrationViewModel ToAccountRegistrationViewModel(this User u, ClientReference[] entRefs) {
            return new AccountRegistrationViewModel {
                UserName = u.UserName,
                UserType = u.UserType,
                //BusinessType = u.BusinessType,
                LinkedEnterprise = u.LinkedEnterprise?.Id ?? -1,
                LinkedEnterpriseName = u.LinkedEnterprise?.Name ?? "Todas",
                FirstName = u.FirstName,
                LastName = u.LastName,
                Id = u.Id,
                Email = u.Email,
                LastAccess = u.LastAccess,
                CanIssuePayments = u.CanIssuePayments ? "1" : "0",
                IsActive = u.IsActive,
                EnterpriseCatalog = entRefs
            };
        }

        public static AccountUpdateViewModel ToAccountUpdateViewModel(this User u, ClientReference[] entRefs) {
            return new AccountUpdateViewModel {
                UserName = u.UserName,
                UserType = u.UserType,
                //BusinessType = u.BusinessType,
                LinkedEnterprise = u.LinkedEnterprise?.Id ?? 0,
                LinkedEnterpriseName = u.LinkedEnterprise?.Name ?? "Todas",
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                CanIssuePayments = u.CanIssuePayments ? "1" : "0",
                EnterpriseCatalog = entRefs
            };
        }

        public static EnterpriseInfoViewModel ToPaydayListViewModel(this Enterprise enterprise) {
            var employeeCount = enterprise.Employees.Count(e => e.Visible);
            return new EnterpriseInfoViewModel {
                Id = enterprise.Id,
                Name = enterprise.Name,
                PayDays = enterprise.PayDays.Select(e => e.ToPayDayViewModel(employeeCount)).ToList()
            };
        }

        public static PayDayReference ToPayDayReference(this PayDay p) {
            return new PayDayReference {
                Id = p.Id,
                Date = p.PayDate.ToString("dd MMM yyyy"),
                Processed = false,
                ProcessedMessage = "",
                Success = false
            };
        }

        public static PayDayViewModel ToPayDayViewModel(this PayDay p, int employees) {
            return new PayDayViewModel {
                Id = p.Id,
                Date = p.PayDate.ToString("dd MMM yyyy"),
                Processed = false,
                ProcessedMessage = "",
                Success = false,
                Authorized = p.Authorizer != null ? "Aprobada" : "En espera",
                Authorizer = p.Authorizer != null ? p.Authorizer.UserName : "",
                Employees = employees,
                StartDate = p.PayStartDate.ToString("dd MMM yyyy"),
                AuthorizationDate = p.Authorizer != null ? p.AuthotizationDate.ToString("dd MMM yyyy") : "",
                EnterpriseId = p.Enterprise.Id
            };
        }

        public static PayDayDetailViewModel ToPayDayDetailViewModel(this EmployeePayDay p) {
            return new PayDayDetailViewModel {
                Name = p.Employee.Name + " " + p.Employee.LastName,
                DailySalary = Math.Round(p.DailySalary, 2).ToString("C"),
                NaturalDays = p.NaturalDays,
                Income = Math.Round(p.Perceptions, 2).ToString("C"),
                Perceptions = Math.Round(0d, 2).ToString("C"),
                Deductions = Math.Round(p.Deductions, 2).ToString("C"),
                FinalIncome = Math.Round((p.Perceptions - p.Deductions), 2).ToString("C"),
                Department = p.Employee.Department.Name
            };
        }

        public static EmployeePayDayFullInfo ToPayDayFullDetailViewModel(this EmployeePayDay p) {
            var maxImss = 1301.025;
            var minImss = 50.91;
            var imssDeduction = p.Employee.HasSocialSecurity ? Math.Min(Math.Max((0.02375 * p.Perceptions), minImss), maxImss) : 0;
            var isrDeduction = p.Deductions - imssDeduction;
            return new EmployeePayDayFullInfo {
                Name = p.Employee.Name + " " + p.Employee.LastName,
                DailySalary = Math.Round(p.DailySalary, 2).ToString("C"),
                NaturalDays = p.NaturalDays,
                Income = Math.Round(p.Perceptions, 2).ToString("C"),
                Perceptions = Math.Round(0d, 2).ToString("C"),
                Deductions = Math.Round(p.Deductions, 2).ToString("C"),
                FinalIncome = Math.Round((p.Perceptions - p.Deductions), 2).ToString("C"),
                Department = p.Employee.Department.Name,
                Vacations = Math.Round(p.Vacations, 2).ToString("C"),
                BreakDays= Math.Round(p.BreakDays, 2).ToString("C"),
                Holidays= Math.Round(p.Holidays, 2).ToString("C"),
                DoublePay= Math.Round(p.DoublePay, 2).ToString("C"),
                TriplePay= Math.Round(p.TriplePay, 2).ToString("C"),
                Overtime= Math.Round(p.Overtime, 2).ToString("C"),
                SundayPrime= Math.Round(0d, 2).ToString("C"),
                VacationPrime= Math.Round(p.VacationPrime, 2).ToString("C"),
                ImssDeduction = Math.Round(imssDeduction, 2).ToString("C"),
                IsrDeduction = Math.Round(isrDeduction, 2).ToString("C"),
            };
        }

        public static List<PayDayDetailViewModel> ToPayDayDepartmentViewModel(this IQueryable<EmployeePayDay> p) {
            var list = new List<PayDayDetailViewModel>();
            var paydays = p.GroupBy(l => l.Employee.Department.Name);
            foreach (var payday in paydays) {
                list.Add(new PayDayDetailViewModel {
                    Name = payday.Key,
                    DailySalary = Math.Round(payday.Sum(d => d.DailySalary), 2).ToString("C"),
                    NaturalDays = payday.First().NaturalDays,
                    Income = Math.Round(payday.Sum(d => d.Perceptions), 2).ToString("C"),
                    Perceptions = Math.Round(0d, 2).ToString("C"),
                    Deductions = Math.Round(payday.Sum(d => d.Deductions), 2).ToString("C"),
                    FinalIncome = Math.Round(payday.Sum(d => d.Perceptions - d.Deductions), 2).ToString("C"),
                    Department = payday.Key,
                    
                });
            }
            return list;
        }
    }
}