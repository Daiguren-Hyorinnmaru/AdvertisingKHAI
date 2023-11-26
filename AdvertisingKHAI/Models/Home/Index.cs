namespace AdvertisingKHAI.Models.Home
{
    public class Index
    {
        public string[] CategoryNames { get; set; }
        public List<List<string>> BannerContent { get; set; }

        public Index(string[] categoryName, List<List<string>> bannerContent)
        {
            CategoryNames = categoryName;
            BannerContent = bannerContent;
        }
    }
}
