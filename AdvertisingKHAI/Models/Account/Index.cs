namespace AdvertisingKHAI.Models.Account
{
    public class Index
    {
        public List<string> CategoryNames { get; set; }
        public List<List<string>> BannerData { get; set; }
        public List<List<int>> BannersIds { get; set; }

        public Index(List<string> categoryNames, List<List<string>> bannerData, List<List<int>> bannersIds)
        {
            CategoryNames = categoryNames;
            BannerData = bannerData;
            BannersIds = bannersIds;
        }
    }
}
