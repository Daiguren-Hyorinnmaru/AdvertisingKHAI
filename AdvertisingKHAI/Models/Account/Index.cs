namespace AdvertisingKHAI.Models.Account
{
    public class Index
    {
        public List<string> CategoryNames { get; set; }
        public List<List<string>> BannerData { get; set; }

        public Index(List<string> categoryNames, List<List<string>> bannerData)
        {
            CategoryNames = categoryNames;
            BannerData = bannerData;
        }
    }
}
