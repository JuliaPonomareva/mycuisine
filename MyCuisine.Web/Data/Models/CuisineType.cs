using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class CuisineType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public List<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
    public static class CuisineTypeExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<CuisineType>().Property(x => x.Name).HasMaxLength(100).IsRequired(true);

            builder.Entity<CuisineType>()
                .HasIndex(entry => entry.Name)
                .IsUnique(true)
                .HasDatabaseName("IX_CuisineTypes_Name");

            builder.Entity<CuisineType>().ToTable("CuisineTypes");
        }
    }
}
