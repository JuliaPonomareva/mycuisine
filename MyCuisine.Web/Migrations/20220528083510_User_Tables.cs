using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCuisine.Web.Migrations
{
    public partial class User_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRecipes_UserId",
                table: "UserRecipes");

            migrationBuilder.DropIndex(
                name: "IX_RecipeRates_UserId",
                table: "RecipeRates");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "UserRecipes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserRecipes");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "UserRecipes");

            migrationBuilder.CreateIndex(
                name: "IX_UserRecipes_UserId_RecipeId",
                table: "UserRecipes",
                columns: new[] { "UserId", "RecipeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeRates_UserId_RecipeId",
                table: "RecipeRates",
                columns: new[] { "UserId", "RecipeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRecipes_UserId_RecipeId",
                table: "UserRecipes");

            migrationBuilder.DropIndex(
                name: "IX_RecipeRates_UserId_RecipeId",
                table: "RecipeRates");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateModified",
                table: "UserRecipes",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserRecipes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "UserRecipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRecipes_UserId",
                table: "UserRecipes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeRates_UserId",
                table: "RecipeRates",
                column: "UserId");
        }
    }
}
