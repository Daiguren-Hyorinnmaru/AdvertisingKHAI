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

                Company? company = _context.Companies
                    .Include(c => c.Category)
                    .SingleOrDefault(c => c.Name == UserName);

                if (company != null)
                {
                    List<string> categoryNames = new();
                    List<List<Banner>> bannerContent = new();
                    categoryNames = company.Category.Select(c => c.Name).ToList();

                    if (categoryNames != null)
                    {
                        foreach (var categoryName in categoryNames)
                        {
                            bannerContent.Add(_context.Banners
                                .Where(b => b.Company != null && b.Category != null && b.Company.Name == UserName && b.Category.Name == categoryName)
                                .ToList());
                        }

                        List<List<string>> bannerData = new();

                        foreach (List<Banner> categoryBanner in bannerContent)
                        {
                            List<string> bannerCategoryString64Data = new();

                            foreach (Banner banner in categoryBanner)
                            {
                                //convert string for browser
                                string base64String = "data:image/jpg;base64," + Convert.ToBase64String(banner.ImageData);
                                bannerCategoryString64Data.Add(base64String);
                            }
                            bannerData.Add(bannerCategoryString64Data);
                        }

                        Models.Account.Index model = new(categoryNames, bannerData);

                        return View(model);
                    }
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
                Company? company = _context.Companies
                    .Include(c => c.Category)
                    .SingleOrDefault(c => c.Name == UserName);

                if (company != null)
                {
                    Category? categoryToRemove = company.Category.SingleOrDefault(c => c.Name == category.CategoryName);

                    if (categoryToRemove != null)
                    {
                        //search for all company banners in a category
                        List<Banner> bannersToDelete =
                            _context.Banners.Where(b => b.CategoryID == categoryToRemove.ID && b.CompanyID == company.ID).ToList();

                        //remuve list banners
                        foreach (Banner banner in bannersToDelete)
                        {
                            Console.WriteLine(banner.ImageName);
                            categoryToRemove.Banners.Remove(banner);
                        }

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
                        //convert data banner to db
                        string prefixToRemove = "data:image/jpeg;base64,";
                        if (!banner.ImageData.StartsWith(prefixToRemove))
                            prefixToRemove = "data:image/jpg;base64,";
                        banner.ImageData = banner.ImageData[prefixToRemove.Length..];
                        byte[] byteArray = Convert.FromBase64String(banner.ImageData);

                        Banner newBanner = new()
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
            byte[] byteArray;
            string? UserName = User?.Identity?.Name;
            //convert data banner to db
            {
                string prefixToRemove = "data:image/jpeg;base64,";
                if (!banner.ImageData.StartsWith(prefixToRemove))
                    prefixToRemove = "data:image/jpg;base64,";
                banner.ImageData = banner.ImageData[prefixToRemove.Length..];
                byteArray = Convert.FromBase64String(banner.ImageData);
            }
            Banner? deleteBanner = _context.Banners
                .SingleOrDefault(c => c.Category != null && c.Category.Name == banner.CategoryName && c.ImageData == byteArray);

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
            if (companyInfo.Count == 0)
            {
                return BadRequest();
            }

            List<string> strings = new();

            foreach (Claim claim in companyInfo)
            {
                List<string> claimType = claim.Type.Split('/').ToList();
                strings.Add(claimType.Last() + ": " + claim.Value.ToString());
            }
            return Json(strings);
        }
    }
}
