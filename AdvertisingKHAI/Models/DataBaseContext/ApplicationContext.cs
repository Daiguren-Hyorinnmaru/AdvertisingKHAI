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


        public void ReBild()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();

            List<Category> categorys = new();
            categorys.Add(new Category { Name = "черный-красный-темн.красный" });
            categorys.Add(new Category { Name = "желтый-синий-зеленый" });
            categorys.Add(new Category { Name = "темн.зеленый-фиолетовый-темн.розовый" });
            Categories.AddRange(categorys);
            SaveChanges();

            Company companyTest = new Company
            {
                Name = "test",
                Email = "test@t",
                PhoneNumber = "+380996847850",
                Password = "qwer",
                Category = categorys
            };
            Companies.Add(companyTest);
            SaveChanges();

            Category? category;
            Company? company= Companies.FirstOrDefault(c => c.Name == "test");

            if (company != null)
            {
                category = Categories.FirstOrDefault(c => c.Name == "черный-красный-темн.красный");
                if (category != null)
                {
                    byte[] imageData1 = File.ReadAllBytes("wwwroot/banner1.jpg");
                    Banner baner1 = new Banner
                    {
                        ImageData = imageData1,
                        ImageName = "banner1.jpg",
                        CategoryID = category.ID,
                        CompanyID = company.ID
                    };
                    Banners.Add(baner1);
                    SaveChanges();
                }

                category = Categories.FirstOrDefault(c => c.Name == "желтый-синий-зеленый");
                if (category != null)
                {
                    byte[] imageData2 = File.ReadAllBytes("wwwroot/banner2.jpg");
                    Banner baner2 = new Banner
                    {
                        ImageData = imageData2,
                        ImageName = "banner2.jpg",
                        CategoryID = category.ID,
                        CompanyID = company.ID
                    };
                    Banners.Add(baner2);
                    SaveChanges();
                }

                category = Categories.FirstOrDefault(c => c.Name == "темн.зеленый-фиолетовый-темн.розовый");
                if (category != null)
                {
                    byte[] imageData3 = File.ReadAllBytes("wwwroot/banner3.jpg");
                    Banner baner3 = new Banner
                    {
                        ImageData = imageData3,
                        ImageName = "banner3.jpg",
                        CategoryID = category.ID,
                        CompanyID = company.ID
                    };
                    Banners.Add(baner3);
                    SaveChanges();
                }


                category = Categories.FirstOrDefault(c => c.Name == "темн.зеленый-фиолетовый-темн.розовый");
                if (category != null)
                {
                    byte[] imageData1_1 = File.ReadAllBytes("wwwroot/banner1_1.jpg");
                    Banner baner1_1 = new Banner
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
