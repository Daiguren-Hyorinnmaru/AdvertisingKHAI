using AdvertisingKHAI.Models;
using AdvertisingKHAI.Models.DataBaseContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;

namespace AdvertisingKHAI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;

        public HomeController(ApplicationContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Отримання масиву categoryNames з джерела даних (наприклад, з бази даних)
            string[] categoryNames = _context.Categories.Select(c => c.Name).ToArray();

            List<List<byte[]>> bannerContent = _context.Categories
            .Include(c => c.Banners)
            .Select(c => c.Banners.Select(b => b.ImageData).ToList())
            .ToList();

            List<List<string>> bannerContentTo = new List<List<string>>();

            foreach (var innerList in bannerContent)
            {
                List<string> convertedInnerList = new List<string>();
                foreach (var item in innerList)
                {
                    string base64String = "data:image/png;base64," + Convert.ToBase64String(item);
                    convertedInnerList.Add(base64String);
                }
                bannerContentTo.Add(convertedInnerList);
            }

            Models.Home.Index model = new(categoryNames, bannerContentTo);

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
