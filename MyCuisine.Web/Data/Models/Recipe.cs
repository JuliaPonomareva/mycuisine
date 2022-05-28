using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? PersonsCount { get; set; }
        public string Image { get; set; }
        public float Rate { get; set; }
        public int Votes { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int DishTypeId { get; set; }
        public DishType DishType { get; set; }

        public int CuisineTypeId { get; set; }
        public CuisineType CuisineType { get; set; }

        public List<RecipeItem> RecipeItems { get; set; } = new List<RecipeItem>();
        public List<RecipeOtherProperty> RecipesOtherProperties { get; set; } = new List<RecipeOtherProperty>();
        public List<UserRecipe> UserRecipes { get; set; } = new List<UserRecipe>();
        public List<RecipeRate> RecipeRates { get; set; } = new List<RecipeRate>();
        public List<CookingStep> CookingSteps { get; set; } = new List<CookingStep>();
    }
    public static class RecipeExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<Recipe>().Property(x => x.Name).HasMaxLength(200).IsRequired(true);
            builder.Entity<Recipe>().Property(x => x.PersonsCount).IsRequired(false);

            builder.Entity<Recipe>()
                .HasIndex(entry => entry.Name)
                .IsUnique(true)
                .HasDatabaseName("IX_Recipes_Name");

            builder.Entity<Recipe>().ToTable("Recipes");
        }
    }
}
