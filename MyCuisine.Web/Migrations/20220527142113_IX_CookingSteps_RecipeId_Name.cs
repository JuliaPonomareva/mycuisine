using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCuisine.Web.Migrations
{
    public partial class IX_CookingSteps_RecipeId_Name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CookingSteps_Name",
                table: "CookingSteps");

            migrationBuilder.DropIndex(
                name: "IX_CookingSteps_RecipeId",
                table: "CookingSteps");

            migrationBuilder.CreateIndex(
                name: "IX_CookingSteps_RecipeId_Name",
                table: "CookingSteps",
                columns: new[] { "RecipeId", "Name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CookingSteps_RecipeId_Name",
                table: "CookingSteps");

            migrationBuilder.CreateIndex(
                name: "IX_CookingSteps_Name",
                table: "CookingSteps",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CookingSteps_RecipeId",
                table: "CookingSteps",
                column: "RecipeId");
        }
    }
}
