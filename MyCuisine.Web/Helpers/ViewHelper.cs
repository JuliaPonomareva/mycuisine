using Microsoft.AspNetCore.Html;

namespace MyCuisine.Web.Helpers
{
    public static class ViewHelper
    {
        public static HtmlString GetBooleanElement(bool value)
        {
            return value
                ? new HtmlString("<i class=\"bi bi-check-lg text-success\"></i>")
                : new HtmlString("<i class=\"bi bi-x-lg text-danger\"></i>");
        }

        public static string TimeAgo(DateTimeOffset dt)
        {
            TimeSpan span = DateTimeOffset.Now - dt;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("около {0} {1} назад",
                years, years == 1 ? "года" : "лет");
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("около {0} {1} назад",
                months, months == 1 ? "месяца" : "месяцев");
            }
            if (span.Days > 0)
                return String.Format("около {0} {1} назад",
                span.Days, span.Days == 1 ? "дня" : "дней");
            if (span.Hours > 0)
                return String.Format("около {0} {1} назад",
                span.Hours, span.Hours == 1 ? "часа" : "часов");
            if (span.Minutes > 0)
                return String.Format("около {0} {1} назад",
                span.Minutes, span.Minutes == 1 ? "минуты" : "минут");
            if (span.Seconds > 5)
                return String.Format("около {0} секунд назад", span.Seconds);
            if (span.Seconds <= 5)
                return "только что";
            return string.Empty;
        }
    }
}
