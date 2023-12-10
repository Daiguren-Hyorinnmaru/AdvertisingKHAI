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
            List<string> categoryNames = _context.Categories.Select(c => c.Name).ToList();

            List<List<byte[]>> bannerContent = _context.Categories
            .Include(c => c.Banners)
            .Select(c => c.Banners.Select(b => b.ImageData).ToList())
            .ToList();

            List<List<string>> bannerStringContent = new List<List<string>>();

            foreach (List<byte[]> innerList in bannerContent)
            {
                List<string> convertedInnerList = new List<string>();
                foreach (byte[] item in innerList)
                {
                    string base64String = "data:image/jpg;base64," + Convert.ToBase64String(item);
                    convertedInnerList.Add(base64String);
                }
                //Console.WriteLine(convertedInnerList.Count);
                bannerStringContent.Add(convertedInnerList);
            }

            List<List<string>> bannerContentToModel = new List<List<string>>();
            List<string> categoryNamesToModel = new List<string>();

            for (int i = 0; i < bannerStringContent.Count(); i++)
            {
                if (bannerStringContent[i].Count > 0)
                {
                    bannerContentToModel.Add(bannerStringContent[i]);
                    categoryNamesToModel.Add(categoryNames[i]);
                }
            }

            Models.Home.Index model = new(categoryNamesToModel, bannerContentToModel);

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
