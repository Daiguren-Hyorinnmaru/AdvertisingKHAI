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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(CompanyLogin model)
        {
            if (ModelState.IsValid)
            {
                // Здесь вы можете добавить логику проверки учетных данных пользователя

                Company? company = _context.Companies.FirstOrDefault(c => c.Email == model.Email && c.Name == model.Name && c.Password == model.Password);

                if (company == null)
                {
                    ViewData["ErrorMessage"] = "Користувач не знайден";
                    return View("Login");
                }

                // Если учетные данные верны, создайте утверждения (claims) пользователя
                List<Claim> claims = new()
                {
                     new Claim(ClaimTypes.Name, company.Name),
                     new Claim(ClaimTypes.Email, company.Email),
                     new Claim(ClaimTypes.MobilePhone, company.PhoneNumber)
                };

                var identity = new ClaimsIdentity(claims, "CookieAuthentication");
                var principal = new ClaimsPrincipal(identity);

                // Войти в систему с использованием аутентификации cookie
                await HttpContext.SignInAsync("CookieAuthentication", principal);

                // Редирект после успешной аутентификации
                return RedirectToAction("Index", "Home");
            }
            ViewData["ErrorMessage"] = "ModelState.IsValid";

            // Если ModelState не валидна, возвращаем пользователя на страницу входа с ошибками
            return View("Login");
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        //почистить метод
        [HttpPost]
        public IActionResult Registration(Company model)
        {
            if (ModelState.IsValid)
            {
                // Используем LINQ для поиска компании по имени
                Company? company = _context.Companies.FirstOrDefault(c => c.Name == model.Name);

                if (company != null)
                {
                    ViewData["ErrorMessage"] = "Компанія з таким ім'ям вже існує.";
                    return View("Registration");
                }
                //Console.WriteLine("model:");
                //Console.WriteLine(model.ID);
                //Console.WriteLine(model.Name);
                //Console.WriteLine(model.Email);
                //Console.WriteLine(model.PhoneNumber);
                //Console.WriteLine(model.Password);
                //Console.WriteLine();

                _context.Companies.Add(model);

                _context.SaveChanges();

                //var users = _context.Companies.ToList();
                //foreach (Company u in users)
                //{
                //    Console.WriteLine($"{u.ID} - {u.Name} - {u.Email} - {u.PhoneNumber} - {u.Password}");
                //}

                // Редирект после успешной регистрации
                return RedirectToAction("Index", "Home");
            }

            // Если ModelState не валидна, возвращаем пользователя на страницу регистрации с ошибками
            return View("Registration");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuthentication");
            return RedirectToAction("Index", "Home");
        }
    }
}
