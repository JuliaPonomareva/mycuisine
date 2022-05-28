namespace MyCuisine.Web.Models
{
    public class Paging
    {
        public Paging(int totalCount, int offset, int limit, Func<int, int, string> getUrl)
        {
            TotalCount = totalCount;
            Offset = offset;
            Limit = limit;
            GetUrl = getUrl;
        }
        public int TotalCount { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public Func<int, int, string> GetUrl { get; set; }
        public int CurrentPage => (Offset / Limit) + 1;
        public int Pages => (int)Math.Ceiling((float)TotalCount / Limit);

        public int CaclOffset(int page, int limit)
        {
            return (page - 1) * limit;
        }
    }

    public class PagingUrls
    {
        public int Number { get; set; }

        public string Url { get; set; }
    }
}
