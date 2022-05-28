using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class QuantityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public List<RecipeItem> RecipeItems { get; set; } = new List<RecipeItem>();
    }
    public static class QuantityTypeExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<QuantityType>().Property(x => x.Name).HasMaxLength(100).IsRequired(true);

            builder.Entity<QuantityType>()
                .HasIndex(entry => entry.Name)
                .IsUnique(true)
                .HasDatabaseName("IX_QuantityTypes_Name");

            builder.Entity<QuantityType>().ToTable("QuantityTypes");
        }
    }
}
