using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIEL.web.Migrations
{
    /// <inheritdoc />
    public partial class purchasetables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<string>(
            //    name: "CategoryName",
            //    table: "Categories",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(100)",
            //    oldMaxLength: 100);

            //migrationBuilder.AddColumn<int>(
            //    name: "MainCategoryId",
            //    table: "Categories",
            //    type: "int",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    varientid = table.Column<int>(type: "int", nullable: false),
                    QuantityOnHand = table.Column<int>(type: "int", nullable: false),
                    AverageCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.InventoryId);
                });

            migrationBuilder.CreateTable(
                name: "InventoryBatch",
                columns: table => new
                {
                    InventoryBatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    varientid = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuantityIn = table.Column<int>(type: "int", nullable: false),
                    QuantityOut = table.Column<int>(type: "int", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryBatch", x => x.InventoryBatchId);
                });

            //migrationBuilder.CreateTable(
            //    name: "MainCategories",
            //    columns: table => new
            //    {
            //        MainCategoryId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MainCategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_MainCategories", x => x.MainCategoryId);
            //    });

            migrationBuilder.CreateTable(
                name: "PurchaseItems",
                columns: table => new
                {
                    PurchaseItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseId = table.Column<int>(type: "int", nullable: false),
                    varientid = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GstPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxableAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseItems", x => x.PurchaseItemId);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseMasters",
                columns: table => new
                {
                    PurchaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalTaxable = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseMasters", x => x.PurchaseId);
                });

            migrationBuilder.CreateTable(
                name: "VariantPrices",
                columns: table => new
                {
                    VariantPriceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    varientid = table.Column<int>(type: "int", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantPrices", x => x.VariantPriceId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_productspecifications_ProductId",
                table: "productspecifications",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProColorSizeVariants_ProductId",
                table: "ProColorSizeVariants",
                column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Categories_MainCategoryId",
            //    table: "Categories",
            //    column: "MainCategoryId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Categories_MainCategories_MainCategoryId",
            //    table: "Categories",
            //    column: "MainCategoryId",
            //    principalTable: "MainCategories",
            //    principalColumn: "MainCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProColorSizeVariants_ProductMasters_ProductId",
                table: "ProColorSizeVariants",
                column: "ProductId",
                principalTable: "ProductMasters",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_productspecifications_ProductMasters_ProductId",
                table: "productspecifications",
                column: "ProductId",
                principalTable: "ProductMasters",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        //    migrationBuilder.DropForeignKey(
        //        name: "FK_Categories_MainCategories_MainCategoryId",
        //        table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProColorSizeVariants_ProductMasters_ProductId",
                table: "ProColorSizeVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_productspecifications_ProductMasters_ProductId",
                table: "productspecifications");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "InventoryBatch");

            //migrationBuilder.DropTable(
            //    name: "MainCategories");

            migrationBuilder.DropTable(
                name: "PurchaseItems");

            migrationBuilder.DropTable(
                name: "PurchaseMasters");

            migrationBuilder.DropTable(
                name: "VariantPrices");

            migrationBuilder.DropIndex(
                name: "IX_productspecifications_ProductId",
                table: "productspecifications");

            migrationBuilder.DropIndex(
                name: "IX_ProColorSizeVariants_ProductId",
                table: "ProColorSizeVariants");

            //migrationBuilder.DropIndex(
            //    name: "IX_Categories_MainCategoryId",
            //    table: "Categories");

            //migrationBuilder.DropColumn(
            //    name: "MainCategoryId",
            //    table: "Categories");

            //migrationBuilder.AlterColumn<string>(
            //    name: "CategoryName",
            //    table: "Categories",
            //    type: "nvarchar(100)",
            //    maxLength: 100,
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)");
        }
    }
}
