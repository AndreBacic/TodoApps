using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

                    var t = HttpContext.User.Identity.Name;

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
        public IActionResult Register(UserViewModel newUser)
        {
            var allUsers = _db.LoadRecords<UserModel>();
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
            string email = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Email).First().Value;
            var user = _db.LoadRecords<UserModel>().Where(x => x.EmailAddress == email).First();
            var displayUser = new UserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                Password = null
            };
            return View(displayUser);
        }
        [Authorize("Auth_Policy")]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult EditAccount(UserViewModel user)
        {
            return View(user);
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
    }
}