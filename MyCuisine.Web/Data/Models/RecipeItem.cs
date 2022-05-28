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

        public int IngridientId { get; set; }
        public Ingridient Ingridient { get; set; }

        public int QuantityTypeId { get; set; }
        public QuantityType QuantityType { get; set; }
    }
    public static class RecipeItemExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<RecipeItem>()
                .HasIndex(entry => new { entry.RecipeId, entry.IngridientId })
                .IsUnique(true)
                .HasDatabaseName("IX_RecipeItems_RecipeId_IngridientId");

            builder.Entity<RecipeItem>().ToTable("RecipeItems");
        }
    }
}
