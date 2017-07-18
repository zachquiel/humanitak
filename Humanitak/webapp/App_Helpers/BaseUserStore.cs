using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SmartAdminMvc.Models;

namespace SmartAdminMvc.App_Helpers {
    public class BaseUserStore : IUserStore<User>,  IUserEmailStore<User>, IUserPasswordStore<User> {
        public void Dispose() {
        }

        public Task CreateAsync(User user) {
            if (user == null) throw new ArgumentNullException("user");

            using (var db = new DataContext()) {
                db.Users.AddOrUpdate(user);
                db.SaveChanges();
            }

            return Task.FromResult<object>(null);
        }

        public Task UpdateAsync(User user) {
            if (user == null) throw new ArgumentNullException("user");

            using (var db = new DataContext()) {
                db.Users.AddOrUpdate(user);
                db.SaveChanges();
            }

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(User user) {
            if (user == null) throw new ArgumentNullException("user");

            using (var db = new DataContext()) {
                if (!db.Users.Any(u => u.Id == user.Id))
                    Task.FromResult<object>(null);
                var us = db.Users.First(u => u.Id == user.Id);
                db.Users.Remove(us);
                db.SaveChanges();
            }
            return Task.FromResult<object>(null);
        }

        public Task<User> FindByIdAsync(string userId) {
            using (var db = new DataContext()) {
                if (!db.Users.Any(u => u.Id == userId))
                    return Task.FromResult<User>(null);
                var user = db.Users.First(u => u.Id == userId);
                return Task.FromResult(user);
            }
        }

        public Task<User> FindByNameAsync(string userName) {
            using (var db = new DataContext()) {
                if (!db.Users.Any(u => u.UserName == userName))
                    return Task.FromResult<User>(null);
                var user = db.Users.First(u => u.UserName == userName);
                return Task.FromResult(user);
            }
        }

        public Task SetEmailAsync(User user, string email) {
            user.Email = email;
            using (var db = new DataContext()) {
                db.Users.AddOrUpdate(user);
                db.SaveChanges();
            }
            return Task.FromResult<object>(null);
        }

        public Task<string> GetEmailAsync(User user) {
            using (var db = new DataContext()) {
                if (!db.Users.Any(u => u.Id == user.Id))
                    return Task.FromResult("");
                var usr = db.Users.First(u => u.Id == user.Id);
                return Task.FromResult(usr.Email);
            }
        }

        public Task<bool> GetEmailConfirmedAsync(User user) {
            return Task.FromResult(true);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed) {
            return Task.FromResult<object>(null);
        }

        public Task<User> FindByEmailAsync(string email) {
            using (var db = new DataContext()) {
                if (!db.Users.Any(u => u.Email == email))
                    return Task.FromResult<User>(null);
                var user = db.Users.First(u => u.Email == email);
                return Task.FromResult(user);
            }
        }

        public Task SetPasswordHashAsync(User user, string passwordHash) {
            user.PasswordHash = passwordHash;
            using (var db = new DataContext()) {
                db.Users.AddOrUpdate(user);
                db.SaveChanges();
            }
            return Task.FromResult<object>(null);
        }

        public Task<string> GetPasswordHashAsync(User user) {
            using (var db = new DataContext()) {
                if (!db.Users.Any(u => u.Id == user.Id))
                    return Task.FromResult("");
                var usr = db.Users.First(u => u.Id == user.Id);
                return Task.FromResult(usr.Email);
            }
        }

        public Task<bool> HasPasswordAsync(User user) {
            using (var db = new DataContext()) {
                if (!db.Users.Any(u => u.Id == user.Id))
                    return Task.FromResult(false);
                var usr = db.Users.First(u => u.Id == user.Id);
                return Task.FromResult(!string.IsNullOrEmpty(usr.PasswordHash));
            }
        }
    }
}