using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCuisine.Web.Migrations
{
    public partial class CookingSteps_IsActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CookingSteps");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CookingSteps",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
