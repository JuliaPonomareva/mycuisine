namespace MyCuisine.Web.Models
{
    public class RecipeDetailInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public float Rate { get; set; }
        public int Votes { get; set; }
        public string DishType { get; set; }
        public string CuisineType { get; set; }
        public List<string> OtherProperties { get; set; } = new List<string>();
    }

    public class RecipeDetailIngridientsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<IngridientViewModel> Ingridients { get; set; } = new List<IngridientViewModel>();
        public class IngridientViewModel
        {
            public string Name { get; set; }
            public string Image { get; set; }
            public bool IsMain { get; set; }
            public int OrderNumber { get; set; }
            public float Quantity { get; set; }
            public string QuantityType { get; set; }
        }
    }

    public class RecipeDetailCookingStepsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CookingStepViewModel> CookingSteps { get; set; } = new List<CookingStepViewModel>();
        public class CookingStepViewModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Image { get; set; }
            public int OrderNumber { get; set; }
        }
    }

    public class RecipeDetailRatesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<RateViewModel> Rates { get; set; } = new List<RateViewModel>();
        public class RateViewModel
        {
            public string User { get; set; }
            public string Rate { get; set; }
            public string Comment { get; set; }
            public DateTimeOffset DateCreated { get; set; }
            public DateTimeOffset DateModified { get; set; }
        }
    }

    public class RecipeDetailRatesAddViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FormModel Form { get; set; }
        public class FormModel
        {
            public string Rate { get; set; }
            public string Comment { get; set; }
        }
    }
}
