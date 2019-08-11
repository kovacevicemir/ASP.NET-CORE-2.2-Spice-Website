using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class modelItemTypoFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_Category_SubCategoryId",
                table: "MenuItem");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_SubCategory_SubCategoryId",
                table: "MenuItem",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_SubCategory_SubCategoryId",
                table: "MenuItem");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_Category_SubCategoryId",
                table: "MenuItem",
                column: "SubCategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
