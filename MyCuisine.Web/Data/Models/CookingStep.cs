using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class CookingStep
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int OrderNumber { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
    public static class CookingStepExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<CookingStep>().Property(x => x.Name).HasMaxLength(100).IsRequired(true);

            builder.Entity<CookingStep>()
                .HasIndex(entry => new { entry.RecipeId, entry.Name })
                .IsUnique(true)
                .HasDatabaseName("IX_CookingSteps_RecipeId_Name");

            builder.Entity<CookingStep>().ToTable("CookingSteps");
        }
    }
}
