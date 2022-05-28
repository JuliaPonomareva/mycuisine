using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class Ingridient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public List<RecipeItem> RecipeItems { get; set; } = new List<RecipeItem>();
    }
    public static class IngridientExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<Ingridient>().Property(x => x.Name).HasMaxLength(100).IsRequired(true);

            builder.Entity<Ingridient>()
                .HasIndex(entry => entry.Name)
                .IsUnique(true)
                .HasDatabaseName("IX_Ingridients_Name");

            builder.Entity<Ingridient>().ToTable("Ingridients");
        }
    }
}
