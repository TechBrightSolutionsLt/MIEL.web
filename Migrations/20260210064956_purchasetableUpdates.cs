using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIEL.web.Migrations
{
    /// <inheritdoc />
    public partial class purchasetableUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProColorSizeVariants_ProductId",
                table: "ProColorSizeVariants");

            //migrationBuilder.RenameColumn(
            //    name: "SizeChartImg",
            //    table: "ProductMasters",
            //    newName: "sizechartPath");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDisc",
                table: "PurchaseMasters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscAmount",
                table: "PurchaseItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscPercent",
                table: "PurchaseItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "size",
                table: "ProColorSizeVariants",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "colour",
                table: "ProColorSizeVariants",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ProColorSizeVariants_ProductId_colour_size",
                table: "ProColorSizeVariants",
                columns: new[] { "ProductId", "colour", "size" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProColorSizeVariants_ProductId_colour_size",
                table: "ProColorSizeVariants");

            migrationBuilder.DropColumn(
                name: "TotalDisc",
                table: "PurchaseMasters");

            migrationBuilder.DropColumn(
                name: "DiscAmount",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "DiscPercent",
                table: "PurchaseItems");

            //migrationBuilder.RenameColumn(
            //    name: "sizechartPath",
            //    table: "ProductMasters",
            //    newName: "SizeChartImg");

            migrationBuilder.AlterColumn<string>(
                name: "size",
                table: "ProColorSizeVariants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "colour",
                table: "ProColorSizeVariants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_ProColorSizeVariants_ProductId",
                table: "ProColorSizeVariants",
                column: "ProductId");
        }
    }
}
