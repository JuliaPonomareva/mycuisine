using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class OtherProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public List<RecipeOtherProperty> RecipesOtherProperties { get; set; } = new List<RecipeOtherProperty>();
    }
    public static class OtherPropertyExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<OtherProperty>().Property(x => x.Name).HasMaxLength(100).IsRequired(true);

            builder.Entity<OtherProperty>()
                .HasIndex(entry => entry.Name)
                .IsUnique(true)
                .HasDatabaseName("IX_OtherProperties_Name");

            builder.Entity<OtherProperty>().ToTable("OtherProperties");
        }
    }
}
