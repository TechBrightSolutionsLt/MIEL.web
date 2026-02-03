using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIEL.web.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        INSERT INTO users_TB
        (
            RoleId,
            FirstName,
            LastName,
            Email,
            MobileNumber,
            Gender,
            Address,
            City,
            Postcode,
            Password,
            CreatedDate
        )
        VALUES
        (
            1,
            'Admin',
            'User',
            'admin',
            '0450095524',
            'Male',
            'HQ',
            'XYZ',
            '10001',
            'miel@123',
            GETDATE()
        )
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
