using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class DishType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public List<Recipe> Recipes { get; set; } = new List<Recipe>();
    }

    public static class DishTypeExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<DishType>().Property(x => x.Name).HasMaxLength(100).IsRequired(true);

            builder.Entity<DishType>()
                .HasIndex(entry => entry.Name)
                .IsUnique(true)
                .HasDatabaseName("IX_DishTypes_Name");

            builder.Entity<DishType>().ToTable("DishTypes");
        }
    }
}
