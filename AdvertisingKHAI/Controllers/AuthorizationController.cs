using AdvertisingKHAI.Models.Authorization;
using AdvertisingKHAI.Models.DataBaseContext;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdvertisingKHAI.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly ApplicationContext _context;

        public AuthorizationController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //почистить метод
        [HttpPost]
        public IActionResult Registration(Company model)
        {
            if (ModelState.IsValid)
            {
                Company? company = _context.Companies.FirstOrDefault(c => c.Name == model.Name);

                if (company != null)
                {
                    ViewData["ErrorMessage"] = "Компанія з таким ім'ям вже існує.";
                    return View("Registration");
                }

                _context.Companies.Add(model);

                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View("Registration");
        }

        [HttpPost]
        public async Task<IActionResult> Login(CompanyLogin model)
        {
            if (ModelState.IsValid)
            {
                Company? company = _context.Companies.FirstOrDefault(c => c.Email == model.Email && c.Name == model.Name && c.Password == model.Password);

                if (company == null)
                {
                    ViewData["ErrorMessage"] = "Користувач не знайден";
                    return View("Login");
                }

                //add user info
                List<Claim> claims = new()
                {
                     new Claim(ClaimTypes.Name, company.Name),
                     new Claim(ClaimTypes.Email, company.Email),
                     new Claim(ClaimTypes.MobilePhone, company.PhoneNumber)
                };

                ClaimsIdentity identity = new(claims, "CookieAuthentication");
                ClaimsPrincipal principal = new(identity);

                //login with using cookie
                await HttpContext.SignInAsync("CookieAuthentication", principal);

                return RedirectToAction("Index", "Home");
            }
            ViewData["ErrorMessage"] = "ModelState.IsValid";

            return View("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuthentication");
            return RedirectToAction("Index", "Home");
        }
    }
}
