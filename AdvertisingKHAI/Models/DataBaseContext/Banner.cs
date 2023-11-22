using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AdvertisingKHAI.Models.DataBaseContext
{
    public class Banner
    {
        public int Id { get; set; }
        public required string ImageName { get; set; }
        public required byte[] ImageData { get; set; }

        public required int CategoryID { get; set; }
        public Category? Category { get; set; }

        public required int CompanyID { get; set; }
        public Company? Company { get; set; }
    }
}
