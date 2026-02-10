using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIEL.web.Migrations
{
    /// <inheritdoc />
    public partial class updateProclvarient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.AddColumn<decimal>(
                name: "AverageCost",
                table: "ProColorSizeVariants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "QuantityOnHand",
                table: "ProColorSizeVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "varientCode",
                table: "ProColorSizeVariants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "BatchNo",
                table: "InventoryBatch",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageCost",
                table: "ProColorSizeVariants");

            migrationBuilder.DropColumn(
                name: "QuantityOnHand",
                table: "ProColorSizeVariants");

            migrationBuilder.DropColumn(
                name: "varientCode",
                table: "ProColorSizeVariants");

            migrationBuilder.AlterColumn<string>(
                name: "BatchNo",
                table: "InventoryBatch",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12);

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AverageCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantityOnHand = table.Column<int>(type: "int", nullable: false),
                    varientid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.InventoryId);
                });
        }
    }
}
