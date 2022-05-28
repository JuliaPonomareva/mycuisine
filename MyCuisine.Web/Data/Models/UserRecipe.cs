using Microsoft.EntityFrameworkCore;

namespace MyCuisine.Data.Web.Models
{
    public class UserRecipe
    {
        public int Id { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
    public static class UserRecipeExtension
    {
        public static void DescribeTable(ModelBuilder builder)
        {
            builder.Entity<UserRecipe>()
                .HasIndex(entry => new { entry.UserId, entry.RecipeId })
                .IsUnique(true)
                .HasDatabaseName("IX_UserRecipes_UserId_RecipeId");

            builder.Entity<UserRecipe>().ToTable("UserRecipes");
        }
    }
}
