using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIEL.web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Categories",
            //    columns: table => new
            //    {
            //        CategoryId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Categories", x => x.CategoryId);
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
            //    name: "ProColorSizeVariants",
            //    columns: table => new
            //    {
            //        varientid = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        colour = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        size = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProColorSizeVariants", x => x.varientid);
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
            //        SizeChartImg = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductMasters", x => x.ProductId);
            //    });

            migrationBuilder.CreateTable(
                name: "productspecifications",
                columns: table => new
                {
                    sId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    specificationvalue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productspecifications", x => x.sId);
                });

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
            //        FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
            //        MobileNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
            //        Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
            //        Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Postcode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
            //        Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_users_TB", x => x.CustomerId);
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

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProductImages_ProductId",
            //    table: "ProductImages",
            //    column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Categories");

            //migrationBuilder.DropTable(
            //    name: "Customers");

            //migrationBuilder.DropTable(
            //    name: "ImageItems");

            //migrationBuilder.DropTable(
            //    name: "ProColorSizeVariants");

            //migrationBuilder.DropTable(
            //    name: "ProductImages");

            migrationBuilder.DropTable(
                name: "productspecifications");

            //migrationBuilder.DropTable(
            //    name: "Specifications");

            //migrationBuilder.DropTable(
            //    name: "Suppliers");

            //migrationBuilder.DropTable(
            //    name: "users_TB");

            //migrationBuilder.DropTable(
            //    name: "ProductMasters");
        }
    }
}
