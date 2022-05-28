using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCuisine.Web.Migrations
{
    public partial class newchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeOtherProperty_OtherProperties_OtherPropertyId",
                table: "RecipeOtherProperty");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeOtherProperty_Recipes_RecipeId",
                table: "RecipeOtherProperty");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipeOtherProperty",
                table: "RecipeOtherProperty");

            migrationBuilder.DropIndex(
                name: "IX_RecipeOtherProperty_RecipeId",
                table: "RecipeOtherProperty");

            migrationBuilder.RenameTable(
                name: "RecipeOtherProperty",
                newName: "RecipesOtherProperties");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeOtherProperty_OtherPropertyId",
                table: "RecipesOtherProperties",
                newName: "IX_RecipesOtherProperties_OtherPropertyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipesOtherProperties",
                table: "RecipesOtherProperties",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RecipesOtherProperties_RecipeId_OtherPropertyId",
                table: "RecipesOtherProperties",
                columns: new[] { "RecipeId", "OtherPropertyId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipesOtherProperties_OtherProperties_OtherPropertyId",
                table: "RecipesOtherProperties",
                column: "OtherPropertyId",
                principalTable: "OtherProperties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipesOtherProperties_Recipes_RecipeId",
                table: "RecipesOtherProperties",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipesOtherProperties_OtherProperties_OtherPropertyId",
                table: "RecipesOtherProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipesOtherProperties_Recipes_RecipeId",
                table: "RecipesOtherProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipesOtherProperties",
                table: "RecipesOtherProperties");

            migrationBuilder.DropIndex(
                name: "IX_RecipesOtherProperties_RecipeId_OtherPropertyId",
                table: "RecipesOtherProperties");

            migrationBuilder.RenameTable(
                name: "RecipesOtherProperties",
                newName: "RecipeOtherProperty");

            migrationBuilder.RenameIndex(
                name: "IX_RecipesOtherProperties_OtherPropertyId",
                table: "RecipeOtherProperty",
                newName: "IX_RecipeOtherProperty_OtherPropertyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipeOtherProperty",
                table: "RecipeOtherProperty",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeOtherProperty_RecipeId",
                table: "RecipeOtherProperty",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeOtherProperty_OtherProperties_OtherPropertyId",
                table: "RecipeOtherProperty",
                column: "OtherPropertyId",
                principalTable: "OtherProperties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeOtherProperty_Recipes_RecipeId",
                table: "RecipeOtherProperty",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
