using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCuisine.Web.Migrations
{
    public partial class uindexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Name",
                table: "Recipes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CookingSteps_Name",
                table: "CookingSteps",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recipes_Name",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_CookingSteps_Name",
                table: "CookingSteps");
        }
    }
}
