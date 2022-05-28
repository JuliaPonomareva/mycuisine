using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class RecipeOtherProperty
    {
        public int Id { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public int OtherPropertyId { get; set; }
        public OtherProperty OtherProperty { get; set; }

    }
    public static class RecipeOtherPropertyExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<RecipeOtherProperty>()
                .HasIndex(entry => new { entry.RecipeId, entry.OtherPropertyId })
                .IsUnique(true)
                .HasDatabaseName("IX_RecipesOtherProperties_RecipeId_OtherPropertyId");

            builder.Entity<RecipeOtherProperty>().ToTable("RecipesOtherProperties");
        }
    }
}
