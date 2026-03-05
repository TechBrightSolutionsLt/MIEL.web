using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIEL.web.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProdId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "col",
                table: "ProdColImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SalesItems_varientid",
                table: "SalesItems",
                column: "varientid");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesItems_ProColorSizeVariants_varientid",
                table: "SalesItems",
                column: "varientid",
                principalTable: "ProColorSizeVariants",
                principalColumn: "varientid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesItems_ProColorSizeVariants_varientid",
                table: "SalesItems");

            migrationBuilder.DropIndex(
                name: "IX_SalesItems_varientid",
                table: "SalesItems");

            migrationBuilder.DropColumn(
                name: "col",
                table: "ProdColImages");
        }
    }
}
