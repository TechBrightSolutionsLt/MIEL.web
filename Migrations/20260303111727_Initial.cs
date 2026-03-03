using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIEL.web.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Cart",
            //    columns: table => new
            //    {
            //        CartId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CustomerId = table.Column<int>(type: "int", nullable: true),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        VariantId = table.Column<int>(type: "int", nullable: false),
            //        ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: false),
            //        Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        GuestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Cart", x => x.CartId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Customers",
            //    columns: table => new
            //    {
            //        CustomerId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        EmailId = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        BuildingName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        BuildingNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Landmark = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Coordinates = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        City = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        State = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Pin = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
            //        GstNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
            //        CreditLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Customers", x => x.CustomerId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ImageItems",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ImageItems", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "InventoryBatch",
            //    columns: table => new
            //    {
            //        InventoryBatchId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        varientid = table.Column<int>(type: "int", nullable: false),
            //        BatchNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        QuantityIn = table.Column<int>(type: "int", nullable: false),
            //        QuantityOut = table.Column<int>(type: "int", nullable: false),
            //        CostPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_InventoryBatch", x => x.InventoryBatchId);
            //    });

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

            //migrationBuilder.CreateTable(
            //    name: "ProductMasters",
            //    columns: table => new
            //    {
            //        ProductId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryId = table.Column<int>(type: "int", nullable: false),
            //        ProductCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //        Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Occasion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        ComboPackage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        HSNNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        SupplierId = table.Column<int>(type: "int", nullable: false),
            //        BarcodeNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        sizechartPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductMasters", x => x.ProductId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PurchaseItems",
            //    columns: table => new
            //    {
            //        PurchaseItemId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        PurchaseId = table.Column<int>(type: "int", nullable: false),
            //        varientid = table.Column<int>(type: "int", nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: false),
            //        Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        BatchNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        GstPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        GstAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        DiscPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        DiscAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        TaxableAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PurchaseItems", x => x.PurchaseItemId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PurchaseMasters",
            //    columns: table => new
            //    {
            //        PurchaseId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        SupplierId = table.Column<int>(type: "int", nullable: false),
            //        InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        TotalDisc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        TotalTaxable = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        TotalTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PurchaseMasters", x => x.PurchaseId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Specifications",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        SpecName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        CategoryId = table.Column<int>(type: "int", nullable: false),
            //        Options = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        OptionType = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Specifications", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Suppliers",
            //    columns: table => new
            //    {
            //        SupplierId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        State = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        City = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            //        PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
            //        Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        GSTIN = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IFSC = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "users_TB",
            //    columns: table => new
            //    {
            //        CustomerId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        RoleId = table.Column<int>(type: "int", nullable: false),
            //        FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //        City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        Postcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_users_TB", x => x.CustomerId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "VariantPrices",
            //    columns: table => new
            //    {
            //        VariantPriceId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        varientid = table.Column<int>(type: "int", nullable: false),
            //        SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_VariantPrices", x => x.VariantPriceId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Wishlist",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        CustomerId = table.Column<int>(type: "int", nullable: true),
            //        GuestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Wishlist", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Categories",
            //    columns: table => new
            //    {
            //        CategoryId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        MainCategoryId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Categories", x => x.CategoryId);
            //        table.ForeignKey(
            //            name: "FK_Categories_MainCategories_MainCategoryId",
            //            column: x => x.MainCategoryId,
            //            principalTable: "MainCategories",
            //            principalColumn: "MainCategoryId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProColorSizeVariants",
            //    columns: table => new
            //    {
            //        varientid = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        colour = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        size = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        QuantityOnHand = table.Column<int>(type: "int", nullable: false),
            //        varientCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        AverageCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProColorSizeVariants", x => x.varientid);
            //        table.ForeignKey(
            //            name: "FK_ProColorSizeVariants_ProductMasters_ProductId",
            //            column: x => x.ProductId,
            //            principalTable: "ProductMasters",
            //            principalColumn: "ProductId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductImages",
            //    columns: table => new
            //    {
            //        ImgId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        ImgPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Flag = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductImages", x => x.ImgId);
            //        table.ForeignKey(
            //            name: "FK_ProductImages_ProductMasters_ProductId",
            //            column: x => x.ProductId,
            //            principalTable: "ProductMasters",
            //            principalColumn: "ProductId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "productspecifications",
            //    columns: table => new
            //    {
            //        sId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        Id = table.Column<int>(type: "int", nullable: false),
            //        specificationvalue = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_productspecifications", x => x.sId);
            //        table.ForeignKey(
            //            name: "FK_productspecifications_ProductMasters_ProductId",
            //            column: x => x.ProductId,
            //            principalTable: "ProductMasters",
            //            principalColumn: "ProductId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_productspecifications_Specifications_Id",
            //            column: x => x.Id,
            //            principalTable: "Specifications",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Orders",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CustomerId = table.Column<int>(type: "int", nullable: false),
            //        SalesId = table.Column<int>(type: "int", nullable: true),
            //        OrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        PayId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        VerifyId = table.Column<int>(type: "int", nullable: true),
            //        BankReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        VerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Orders", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Orders_users_TB_CustomerId",
            //            column: x => x.CustomerId,
            //            principalTable: "users_TB",
            //            principalColumn: "CustomerId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SalesMasters",
            //    columns: table => new
            //    {
            //        SalesId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        SalesDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        InvoiceNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
            //        PaymentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        TotalDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        GstAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        paysts = table.Column<int>(type: "int", nullable: false),
            //        salesmode = table.Column<int>(type: "int", nullable: false),
            //        CustomerId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SalesMasters", x => x.SalesId);
            //        table.ForeignKey(
            //            name: "FK_SalesMasters_users_TB_CustomerId",
            //            column: x => x.CustomerId,
            //            principalTable: "users_TB",
            //            principalColumn: "CustomerId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "ProdColImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VariantId = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdColImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdColImages_ProColorSizeVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ProColorSizeVariants",
                        principalColumn: "varientid",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "SalesItems",
            //    columns: table => new
            //    {
            //        SalesItemId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        SalesId = table.Column<int>(type: "int", nullable: false),
            //        varientid = table.Column<int>(type: "int", nullable: false),
            //        BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: false),
            //        SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        DiscPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        DiscAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SalesItems", x => x.SalesItemId);
            //        table.ForeignKey(
            //            name: "FK_SalesItems_SalesMasters_SalesId",
            //            column: x => x.SalesId,
            //            principalTable: "SalesMasters",
            //            principalColumn: "SalesId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Categories_MainCategoryId",
            //    table: "Categories",
            //    column: "MainCategoryId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Orders_CustomerId",
            //    table: "Orders",
            //    column: "CustomerId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProColorSizeVariants_ProductId_colour_size",
            //    table: "ProColorSizeVariants",
            //    columns: new[] { "ProductId", "colour", "size" },
            //    unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProdColImages_VariantId",
                table: "ProdColImages",
                column: "VariantId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProductImages_ProductId",
            //    table: "ProductImages",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_productspecifications_Id",
            //    table: "productspecifications",
            //    column: "Id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_productspecifications_ProductId",
            //    table: "productspecifications",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SalesItems_SalesId",
            //    table: "SalesItems",
            //    column: "SalesId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SalesMasters_CustomerId",
            //    table: "SalesMasters",
            //    column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Cart");

            //migrationBuilder.DropTable(
            //    name: "Categories");

            //migrationBuilder.DropTable(
            //    name: "Customers");

            //migrationBuilder.DropTable(
            //    name: "ImageItems");

            //migrationBuilder.DropTable(
            //    name: "InventoryBatch");

            //migrationBuilder.DropTable(
            //    name: "Orders");

            migrationBuilder.DropTable(
                name: "ProdColImages");

            //migrationBuilder.DropTable(
            //    name: "ProductImages");

            //migrationBuilder.DropTable(
            //    name: "productspecifications");

            //migrationBuilder.DropTable(
            //    name: "PurchaseItems");

            //migrationBuilder.DropTable(
            //    name: "PurchaseMasters");

            //migrationBuilder.DropTable(
            //    name: "SalesItems");

            //migrationBuilder.DropTable(
            //    name: "Suppliers");

            //migrationBuilder.DropTable(
            //    name: "VariantPrices");

            //migrationBuilder.DropTable(
            //    name: "Wishlist");

            //migrationBuilder.DropTable(
            //    name: "MainCategories");

            //migrationBuilder.DropTable(
            //    name: "ProColorSizeVariants");

            //migrationBuilder.DropTable(
            //    name: "Specifications");

            //migrationBuilder.DropTable(
            //    name: "SalesMasters");

            //migrationBuilder.DropTable(
            //    name: "ProductMasters");

            //migrationBuilder.DropTable(
            //    name: "users_TB");
        }
    }
}
