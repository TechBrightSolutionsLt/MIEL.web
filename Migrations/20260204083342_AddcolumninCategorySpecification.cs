using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIEL.web.Migrations
{
    /// <inheritdoc />
    public partial class AddcolumninCategorySpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OptionType",
                table: "Specifications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionType",
                table: "Specifications");
        }
    }
}
