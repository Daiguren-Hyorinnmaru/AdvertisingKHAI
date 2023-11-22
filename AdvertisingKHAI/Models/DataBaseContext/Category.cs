using System.Reflection;

namespace AdvertisingKHAI.Models.DataBaseContext
{
    public class Category
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public List<Banner> Baners { get; set; } = new();
    }
}
