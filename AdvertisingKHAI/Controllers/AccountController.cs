using AdvertisingKHAI.Models.Account;
using AdvertisingKHAI.Models.DataBaseContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.Design;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace AdvertisingKHAI.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationContext _context;

        public AccountController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            string? UserName = User?.Identity?.Name;
            if (UserName != null)
            {
                List<string> categoryNames = new();
                List<List<Banner>> bannerContent = new();

                Company? company = _context.Companies
                    .Include(c => c.Category)
                    .SingleOrDefault(c => c.Name == UserName);

                if (company != null)
                {
                    categoryNames = company.Category.Select(c => c.Name).ToList();

                    if (categoryNames != null)
                        bannerContent = _context.Categories.Include(c => c.Banners)
                        .Select(c => c.Banners.Where(b => b.Company.Name == UserName).ToList())
                        .ToList();
                }
                List<List<string>> bannerData = new();
                List<List<int>> bannersIds = new();

                foreach (List<Banner> categoryBanner in bannerContent)
                {
                    List<string> bannerCategoryString64Data = new();
                    List<int> bannerCategoryID = new();

                    foreach (Banner banner in categoryBanner)
                    {
                        string base64String = "data:image/jpg;base64," + Convert.ToBase64String(banner.ImageData);
                        bannerCategoryString64Data.Add(base64String);
                        //Console.WriteLine("banner.ImageData " + banner.ImageData+ " - "+Convert.ToBase64String(banner.ImageData));
                        //Console.WriteLine();
                        bannerCategoryID.Add(banner.Id);
                    }
                    bannersIds.Add(bannerCategoryID);
                    bannerData.Add(bannerCategoryString64Data);
                }

                if (categoryNames != null)
                {
                    Models.Account.Index model = new(categoryNames, bannerData, bannersIds);

                    foreach (string name in categoryNames) { Console.WriteLine(name); }

                    return View(model);
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddCategory([FromBody] CategoryModel category)
        {
            string? UserName = User?.Identity?.Name;

            if (UserName != null)
            {
                Company? company = _context.Companies
                    .Include(c => c.Category)
                    .SingleOrDefault(c => c.Name == UserName);

                if (company != null)
                {
                    company.Category.Add(new Category { Name = category.CategoryName });
                    _context.SaveChanges();
                }
            }

            return Ok($"Category added successfully.");
        }

        [HttpPost]
        public IActionResult DeleteCategory([FromBody] CategoryModel category)
        {
            string? UserName = User?.Identity?.Name;

            if (UserName != null)
            {
                //Console.WriteLine("UserName != null");
                Company? company = _context.Companies
                    .Include(c => c.Category)
                    .SingleOrDefault(c => c.Name == UserName);

                if (company != null)
                {
                    //Console.WriteLine("company != null");

                    Category? categoryToRemove = company.Category.SingleOrDefault(c => c.Name == category.CategoryName);

                    if (categoryToRemove != null)
                    {
                        List<Banner> bannersToDelete =
                            _context.Banners.Where(b => b.CategoryID == categoryToRemove.ID && b.CompanyID == company.ID).ToList();
                        //Console.WriteLine("CATEGORY TO REMOVE NAME " + categoryToRemove.Name);
                        //Console.WriteLine("CATEGORY TO REMOVE NAME ID" + categoryToRemove.ID);
                        //Console.WriteLine("COUNT TO DELETE " + bannersToDelete.Count());
                        //Console.WriteLine("REMUVE START");
                        //Console.WriteLine("categoryToRemove != null");

                        foreach (Banner banner in bannersToDelete)
                        {
                            Console.WriteLine(banner.ImageName);
                            categoryToRemove.Banners.Remove(banner);

                        }
                        //Console.WriteLine("REMUVE END");

                        company.Category.Remove(categoryToRemove);

                        _context.SaveChanges();

                        _context.RemoveCategoriesWithoutCompanies();
                    }
                }
            }

            return Ok($"Category delete successfully.");

        }

        [HttpPost]
        public IActionResult AddBanner([FromBody] BannerAddModel banner)
        {
            //Console.WriteLine("AddBanner Start");
            //Console.WriteLine("banner.CategoryName " + banner.CategoryName);
            //Console.WriteLine("banner.ImageName " + banner.ImageName);
            //Console.WriteLine("banner.ImageData " + banner.ImageData);
            string? UserName = User?.Identity?.Name;

            if (UserName != null)
            {
                Company? company = _context.Companies
                    .Include(c => c.Baners)
                    .SingleOrDefault(c => c.Name == UserName);

                if (company != null)
                {
                    Category? category = _context.Categories
                        .Include(c => c.Banners)
                        .SingleOrDefault(c => c.Name == banner.CategoryName);

                    if (category != null)
                    {
                        string prefixToRemove = "data:image/jpeg;base64,";
                        if (!banner.ImageData.StartsWith(prefixToRemove))
                            prefixToRemove = "data:image/jpg;base64,";
                        banner.ImageData = banner.ImageData.Substring(prefixToRemove.Length);
                        byte[] byteArray = Convert.FromBase64String(banner.ImageData);
                        //Console.WriteLine("banner.ImageData Remove " + banner.ImageData);

                        Banner newBanner = new Banner
                        {
                            ImageData = byteArray,
                            ImageName = banner.ImageName,
                            CompanyID = company.ID,
                            CategoryID = category.ID
                        };

                        company.Baners.Add(newBanner);
                        category.Banners.Add(newBanner);

                        _context.SaveChanges();
                    }
                }
            }
            return Ok("Banner added successfully.");
        }

        [HttpPost]
        public IActionResult DeleteBanner([FromBody] BannerDeleteModel banner)
        {
            string? UserName = User?.Identity?.Name;
            string prefixToRemove = "data:image/jpeg;base64,";
            if (!banner.ImageData.StartsWith(prefixToRemove))
                prefixToRemove = "data:image/jpg;base64,";
            banner.ImageData = banner.ImageData.Substring(prefixToRemove.Length);
            byte[] byteArray = Convert.FromBase64String(banner.ImageData);

            Banner? deleteBanner = _context.Banners
                .SingleOrDefault(c => c.Category.Name == banner.CategoryName && c.ImageData == byteArray);

            if (deleteBanner != null)
            {
                _context.Banners.Remove(deleteBanner);
                _context.SaveChanges();
            }

            return Ok("Banner added successfully.");
        }

        [HttpGet]
        public IActionResult GetCompanyInfo()
        {
            List<Claim> companyInfo = User.Claims.ToList();
            if(companyInfo.Count == 0)
            {
                return BadRequest();
            }
            List<string> strings = new List<string>();

            foreach (Claim claim in companyInfo)
            {
                List<string> claimType = claim.Type.Split('/').ToList();
                strings.Add(claimType.Last() + ": " + claim.Value.ToString());
            }
            return Json(strings);
        }
    }
}
