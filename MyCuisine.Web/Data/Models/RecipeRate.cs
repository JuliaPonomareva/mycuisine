using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class RecipeRate
    {
        public int Id { get; set; }
        public float Rate { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
    public static class RecipeRateExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<RecipeRate>().Property(x => x.Comment).HasMaxLength(2000).IsRequired(false);

            builder.Entity<RecipeRate>()
                .HasIndex(entry => new { entry.UserId, entry.RecipeId })
                .IsUnique(true)
                .HasDatabaseName("IX_RecipeRates_UserId_RecipeId");

            builder.Entity<RecipeRate>().ToTable("RecipeRates");
        }
    }
}
