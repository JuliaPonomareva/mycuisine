using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public List<RecipeItem> RecipeItems { get; set; } = new List<RecipeItem>();
    }
    public static class IngredientExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<Ingredient>().Property(x => x.Name).HasMaxLength(100).IsRequired(true);

            builder.Entity<Ingredient>()
                .HasIndex(entry => entry.Name)
                .IsUnique(true)
                .HasDatabaseName("IX_Ingredients_Name");

            builder.Entity<Ingredient>().ToTable("Ingredients");
        }
    }
}
