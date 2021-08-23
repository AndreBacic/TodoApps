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
        public IActionResult Login([FromBody]LoginModel user)
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

                    List<Claim> personClaims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, dbUser.Name),
                        new Claim(ClaimTypes.Email, dbUser.EmailAddress)
                    };

                    List<ClaimsIdentity> claimsIdentities = new List<ClaimsIdentity>()
                    {
                        new ClaimsIdentity(personClaims)
                    };

                    HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentities));

                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    throw new FormatException("Incorrect password hash storage format");
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
            return RedirectToPage("Login");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register([FromBody]UserViewModel newUser)
        {
            UserModel newDbUser = new UserModel
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                EmailAddress = newUser.EmailAddress,
                PasswordHash = HashAndSalter.HashAndSalt(newUser.Password).ToDbString()
            };
            _db.InsertRecord(newDbUser);
            return RedirectToAction("Index", "Todo");
        }

        [Authorize("Auth_Policy")]
        public IActionResult EditAccount()
        {
            string email = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Email).First().Value;
            var user = _db.LoadRecords<UserModel>().Where(x => x.EmailAddress == email);
            return View(user);
        }
        [Authorize("Auth_Policy")]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult EditAccount([FromBody]UserViewModel user)
        {
            return View();
        }
    }
}