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

            migrationBuilder.CreateTable(
                name: "ProductMasters",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Occasion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ComboPackage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HSNNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasters", x => x.ProductId);
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

        //    migrationBuilder.CreateTable(
        //        name: "users_TB",
        //        columns: table => new
        //        {
        //            CustomerId = table.Column<int>(type: "int", nullable: false)
        //                .Annotation("SqlServer:Identity", "1, 1"),
        //            RoleId = table.Column<int>(type: "int", nullable: false),
        //            FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
        //            LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
        //            Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
        //            MobileNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
        //            Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
        //            Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
        //            City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
        //            Postcode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
        //            Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
        //            CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
        //        },
        //        constraints: table =>
        //        {
        //            table.PrimaryKey("PK_users_TB", x => x.CustomerId);
        //        });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Categories");

            //migrationBuilder.DropTable(
            //    name: "ImageItems");

            migrationBuilder.DropTable(
                name: "ProductMasters");

            //migrationBuilder.DropTable(
            //    name: "Specifications");

            //migrationBuilder.DropTable(
            //    name: "users_TB");
        }
    }
}
