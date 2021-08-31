using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using TodoMVCAppAsFastAsICan.Data;
using TodoMVCAppAsFastAsICan.Models;
using TodoMVCAppAsFastAsICan.Security;

namespace TodoMVCAppAsFastAsICan.Controllers
{
    public class AccountController : Controller
    {
        private readonly MongoDbAccessor _db;

        public AccountController(MongoDbAccessor db)
        {
            _db = db;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel user)
        {
            try
            {
                UserModel dbUser = _db.LoadRecords<UserModel>()
                    .Where(u => u.EmailAddress == user.EmailAddress).First();

                PasswordHashModel passwordHash = new PasswordHashModel();
                passwordHash.FromDbString(dbUser.PasswordHash);

                (bool IsPasswordCorrect, _) = HashAndSalter.PasswordEqualsHash(user.Password, passwordHash);

                if (IsPasswordCorrect)
                {
                    LogInUser(dbUser);

                    string t = HttpContext.User.Identity.Name;

                    return RedirectToAction("Index", "Todo");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        [Authorize("Auth_Policy")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult Register()
        {
            ViewData["RegisterMessage"] = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel newUser)
        {
            if (ModelState.IsValid == false)
            {
                ViewData["RegisterMessage"] = "Invalid Inputs";
                return View(newUser);
            }

            List<UserModel> allUsers = _db.LoadRecords<UserModel>();
            if (allUsers.Any(x => x.EmailAddress == newUser.EmailAddress))
            {
                ViewData["RegisterMessage"] = "That email address is taken";
                return View();
            }
            UserModel newDbUser = new UserModel
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                EmailAddress = newUser.EmailAddress,
                PasswordHash = HashAndSalter.HashAndSalt(newUser.Password).ToDbString()
            };
            _db.InsertRecord(newDbUser);

            LogInUser(newDbUser);

            return RedirectToAction("Index", "Todo");
        }

        [Authorize("Auth_Policy")]
        public IActionResult EditAccount()
        {
            UserModel user = GetLoggedInUserByEmail();

            return View(DbUserToEditView(user));
        }
        [Authorize("Auth_Policy")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAccount(EditUserViewModel updatedUser)
        {
            // 1) Make sure email isn't taken
            List<UserModel> allUsers = _db.LoadRecords<UserModel>();
            UserModel loggedInUser = GetLoggedInUserByEmail();

            if (IsValidEmailAddress(updatedUser.EmailAddress) == false ||
                allUsers.Any(x => x.EmailAddress == updatedUser.EmailAddress && updatedUser.EmailAddress != loggedInUser.EmailAddress))
            {
                ViewData["EditMessage"] = "That email address is taken";
                return View(DbUserToEditView(loggedInUser));
            }

            if (string.IsNullOrWhiteSpace(updatedUser.NewPassword) == false)
            {
                // 2) Make sure old password is correct
                PasswordHashModel passwordHash = new PasswordHashModel();
                passwordHash.FromDbString(loggedInUser.PasswordHash);

                (bool IsPasswordCorrect, _) = HashAndSalter.PasswordEqualsHash(updatedUser.OldPassword, passwordHash);

                if (IsPasswordCorrect)
                {
                    loggedInUser.FirstName = updatedUser.FirstName;
                    loggedInUser.LastName = updatedUser.LastName;
                    loggedInUser.EmailAddress = updatedUser.EmailAddress;
                    loggedInUser.PasswordHash = HashAndSalter.HashAndSalt(updatedUser.NewPassword).ToDbString();
                    _db.UpsertRecord(loggedInUser.Id, loggedInUser);

                    LogInUser(loggedInUser);

                    loggedInUser.EmailAddress = "";
                    loggedInUser.PasswordHash = "";
                    return RedirectToAction("Index", "Todo");
                }
                else
                {
                    return View(DbUserToEditView(loggedInUser));
                }
            }
            else
            {
                // No password change
                loggedInUser.FirstName = updatedUser.FirstName;
                loggedInUser.LastName = updatedUser.LastName;
                loggedInUser.EmailAddress = updatedUser.EmailAddress;
                _db.UpsertRecord(loggedInUser.Id, loggedInUser);

                LogInUser(loggedInUser);

                return RedirectToAction("Index", "Todo");
            }
        }

        private async void LogInUser(UserModel user)
        {
            List<Claim> personClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.EmailAddress)
                };

            List<ClaimsIdentity> claimsIdentities = new List<ClaimsIdentity>()
                {
                    new ClaimsIdentity(personClaims, "TodoAuth.Identity")
                };

            await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentities));
        }

        private UserModel GetLoggedInUserByEmail()
        {
            string email = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Email).First().Value;
            return _db.LoadRecords<UserModel>().Where(x => x.EmailAddress == email).First();
        }
        private bool IsValidEmailAddress(string emailAddress)
        {
            try
            {
                System.Net.Mail.MailAddress m = new System.Net.Mail.MailAddress(emailAddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private EditUserViewModel DbUserToEditView(UserModel user)
        {
            return new EditUserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress
            };
        }
    }
}