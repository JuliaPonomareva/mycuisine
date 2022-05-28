using MyCuisine.Web.Models;

namespace MyCuisine.Web.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetError(this HttpContext context)
        {
            return context.Items.TryGetValue("Error", out object error) ? (string)error : null;
        }

        public static void SetError(this HttpContext context, string error)
        {
            context.Items["Error"] = error;
        }
    }
}
