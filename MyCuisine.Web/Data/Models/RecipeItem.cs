using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class RecipeItem
    {
        public int Id { get; set; }
        public float Quantity { get; set; }
        public bool IsMain { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
        public int OrderNumber { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }

        public int QuantityTypeId { get; set; }
        public QuantityType QuantityType { get; set; }
    }
    public static class RecipeItemExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<RecipeItem>()
                .HasIndex(entry => new { entry.RecipeId, entry.IngredientId })
                .IsUnique(true)
                .HasDatabaseName("IX_RecipeItems_RecipeId_IngredientId");

            builder.Entity<RecipeItem>().ToTable("RecipeItems");
        }
    }
}
