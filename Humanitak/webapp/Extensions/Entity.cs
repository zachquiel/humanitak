using System.Collections.Generic;
using SmartAdminMvc.Models;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Extensions {
    public static partial class Extensions {
        public static EnterpriseViewModel ToEnterpriseViewModel(this Enterprise enterprise) {
            return new EnterpriseViewModel {
                Id = enterprise.Id,
                Name = enterprise.Name,
                Payday1Start = enterprise.Payday1Start,
                Payday1End = enterprise.Payday1End,
                Payday2Start = enterprise.Payday2Start,
                Payday2End = enterprise.Payday2End,
                LogoImage = enterprise.Logo.Image,
                HeaderImage = enterprise.Header.Image,
                UsesPunchClock = enterprise.UsesPunchClock ? "1" : "0",
                Commission = enterprise.Commission,
                Vat = enterprise.Vat,
                IsActive = enterprise.IsActive ? "1" : "0",
                ParentEnterprise = enterprise.ParentEnterprise?.Id ?? 0,
                Operations = enterprise.Operations,
                City = enterprise.City,
                LastPayday = enterprise.LastPayday,
                State = enterprise.State
            };
        }

        public static EnterpriseInsertViewModel ToEnterpriseInsertViewModel(this Enterprise enterprise, List<EnterpriseReference> references) {
            return new EnterpriseInsertViewModel {
                Id = enterprise.Id,
                Name = enterprise.Name,
                Payday1Start = enterprise.Payday1Start,
                Payday1End = enterprise.Payday1End,
                Payday2Start = enterprise.Payday2Start,
                Payday2End = enterprise.Payday2End,
                LogoImage = enterprise.Logo.Image,
                HeaderImage = enterprise.Header.Image,
                UsesPunchClock = enterprise.UsesPunchClock ? "1" : "0",
                Commission = enterprise.Commission,
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

        public static AccountRegistrationViewModel ToAccountRegistrationViewModel(this User u) {
            return new AccountRegistrationViewModel {
                UserName = u.UserName,
                UserType = u.UserType,
                BusinessType = u.BusinessType,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Id = u.Id,
                Email = u.Email,
                LastAccess = u.LastAccess,
                CanIssuePayments = u.CanIssuePayments ? "1" : "0",
                IsActive = u.IsActive
            };
        }

        public static AccountUpdateViewModel ToAccountUpdateViewModel(this User u) {
            return new AccountUpdateViewModel {
                UserName = u.UserName,
                UserType = u.UserType,
                BusinessType = u.BusinessType,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                CanIssuePayments = u.CanIssuePayments ? "1" : "0"
            };
        }
    }
}