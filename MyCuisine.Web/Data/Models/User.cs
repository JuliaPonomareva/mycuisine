using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public List<UserRecipe> UserRecipes { get; set; } = new List<UserRecipe>();
        public List<RecipeRate> RecipeRates { get; set; } = new List<RecipeRate>();
    }
    public static class UserExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<User>().Property(x => x.Email).HasMaxLength(250).IsRequired(true);
            builder.Entity<User>().Property(x => x.Name).HasMaxLength(50).IsRequired(true);

            builder.Entity<User>()
                .HasIndex(entry => entry.Email)
                .IsUnique(true)
                .HasDatabaseName("IX_Users_Email");

            builder.Entity<User>().ToTable("Users");
        }
    }
}
