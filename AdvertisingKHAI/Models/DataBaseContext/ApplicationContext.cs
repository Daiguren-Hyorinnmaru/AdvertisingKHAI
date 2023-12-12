using Microsoft.EntityFrameworkCore;

namespace AdvertisingKHAI.Models.DataBaseContext
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<Banner> Banners { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=helloapp.db");
        }

        //deletes all categories if they are not in one of the companies
        public void RemoveCategoriesWithoutCompanies()
        {
            var categoriesWithoutCompanies = Categories
                .Where(category => !Companies.Any(company => company.Category.Contains(category)))
                .ToList();

            Categories.RemoveRange(categoriesWithoutCompanies);
            SaveChanges();
        }

        //rebild db and add data for test
        public void ReBild()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();

            List<Category> categorys = new()
            {
                new Category { Name = "Test 1" },
                new Category { Name = "Test 2" },
                new Category { Name = "Test 3" },
                new Category { Name = "baseTest" }
            };
            Categories.AddRange(categorys);
            SaveChanges();

            Company companyTest = new()
            {
                Name = "test",
                Email = "test@t",
                PhoneNumber = "+380996847850",
                Password = "qwerqwer",
                Category = categorys
            };
            Companies.Add(companyTest);
            SaveChanges();

            Category? category;
            Company? company= Companies.FirstOrDefault(c => c.Name == "test");

            if (company != null)
            {
                category = Categories.FirstOrDefault(c => c.Name == "Test 1");
                if (category != null)
                {
                    byte[] imageData1 = File.ReadAllBytes("wwwroot/banner1.jpg");
                    Banner baner1 = new()
                    {
                        ImageData = imageData1,
                        ImageName = "banner1.jpg",
                        CategoryID = category.ID,
                        CompanyID = company.ID
                    };
                    Banners.Add(baner1);
                    SaveChanges();
                }

                category = Categories.FirstOrDefault(c => c.Name == "Test 2");
                if (category != null)
                {
                    byte[] imageData2 = File.ReadAllBytes("wwwroot/banner2.jpg");
                    Banner baner2 = new()
                    {
                        ImageData = imageData2,
                        ImageName = "banner2.jpg",
                        CategoryID = category.ID,
                        CompanyID = company.ID
                    };
                    Banners.Add(baner2);
                    SaveChanges();
                }

                category = Categories.FirstOrDefault(c => c.Name == "Test 1");
                if (category != null)
                {
                    byte[] imageData3 = File.ReadAllBytes("wwwroot/banner3.jpg");
                    Banner baner3 = new()
                    {
                        ImageData = imageData3,
                        ImageName = "banner3.jpg",
                        CategoryID = category.ID,
                        CompanyID = company.ID
                    };
                    Banners.Add(baner3);
                    SaveChanges();
                }


                category = Categories.FirstOrDefault(c => c.Name == "Test 3");
                if (category != null)
                {
                    byte[] imageData1_1 = File.ReadAllBytes("wwwroot/banner1_1.jpg");
                    Banner baner1_1 = new()
                    {
                        ImageData = imageData1_1,
                        ImageName = "banner1_1.jpg",
                        CategoryID = category.ID,
                        CompanyID = company.ID
                    };
                    Banners.Add(baner1_1);
                    SaveChanges();
                }
            }
        }
    }
}
