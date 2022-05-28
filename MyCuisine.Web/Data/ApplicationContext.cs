using Microsoft.EntityFrameworkCore;
using MyCuisine.Data.Web.Models;

namespace MyCuisine.Web.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeItem> RecipeItems { get; set; }
        public DbSet<QuantityType> QuantityTypes { get; set; }
        public DbSet<Ingridient> Ingridients { get; set; }
        public DbSet<DishType> DishTypes { get; set; }
        public DbSet<CuisineType> CuisineTypes { get; set; }
        public DbSet<OtherProperty> OtherProperties { get; set; }
        public DbSet<RecipeOtherProperty> RecipeOtherProperties { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRecipe> UserRecipes { get; set; }
        public DbSet<RecipeRate> RecipeRates { get; set; }
        public DbSet<CookingStep> CookingSteps { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            RecipeExtension.DescribeTable(modelBuilder);
            RecipeItemExtension.DescribeTable(modelBuilder);
            QuantityTypeExtension.DescribeTable(modelBuilder);
            IngridientExtension.DescribeTable(modelBuilder);
            DishTypeExtension.DescribeTable(modelBuilder);
            CuisineTypeExtension.DescribeTable(modelBuilder);
            OtherPropertyExtension.DescribeTable(modelBuilder);
            RecipeOtherPropertyExtension.DescribeTable(modelBuilder);
            UserExtension.DescribeTable(modelBuilder);
            UserRecipeExtension.DescribeTable(modelBuilder);
            RecipeRateExtension.DescribeTable(modelBuilder);
            CookingStepExtension.DescribeTable(modelBuilder);
        }
    }
}
