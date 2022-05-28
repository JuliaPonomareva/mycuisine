using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyCuisine.Web.Models
{
    public class FilterViewModel
    {
        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem("Актуальность", SortingOption.Relevant.ToString()),
            new SelectListItem("Рейтинг", SortingOption.Rate.ToString()),
            new SelectListItem("Голосов", SortingOption.Vote.ToString()),
            new SelectListItem("Алфавит", SortingOption.Alphabetical.ToString()),
        };
        public List<SelectListItem> DishTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CuisineTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> OtherProperties { get; set; } = new List<SelectListItem>();
        public FormModel Form { get; set; }

        public class FormModel
        {
            public SortingOption SortBy { get; set; }
            public List<int> DishTypeIds { get; set; }
            public List<int> CuisineTypeIds { get; set; }
            public List<int> OtherPropertyIds { get; set; }
        }
    }
    public enum SortingOption
    {
        Relevant,
        Rate,
        Vote,
        Alphabetical,
    }
    public class SummaryRecipesViewModel
    {
        public FilterViewModel Filter { get; set; }
        public List<SummaryRecipeViewModel> Entries { get; set; } = new List<SummaryRecipeViewModel>();
        public class SummaryRecipeViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Image { get; set; }
            public float Rate { get; set; }
            public int Votes { get; set; }
            public int DishTypeId { get; set; }
            public string DishType { get; set; }
            public int CuisineTypeId { get; set; }
            public string CuisineType { get; set; }
            public List<OptionViewModel> OtherProperties { get; set; } = new List<OptionViewModel>();
            public DateTimeOffset DateModified { get; set; }
            public bool IsSaved { get; set; }
        }
    }
}
