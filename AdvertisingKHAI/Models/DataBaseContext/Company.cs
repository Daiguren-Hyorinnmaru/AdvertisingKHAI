using System.Reflection;

namespace AdvertisingKHAI.Models.DataBaseContext
{
    public class Company
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public List<Banner> Baners { get; set; } = new();
        public List<Category> Category { get; set; } = new();
    }
}
