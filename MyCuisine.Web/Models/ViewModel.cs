namespace MyCuisine.Web.Models
{
    public class ViewModel<T>
    {
        public Paging Paging { get; set; }
        public T Data { get; set; }
    }
}
