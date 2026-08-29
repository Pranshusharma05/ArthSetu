using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArthSetuBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddGovernmentLevelAndApplicability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicableDistrict",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicableStateUT",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeographicApplicabilityType",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernmentLevel",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicableDistrict",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "ApplicableStateUT",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "GeographicApplicabilityType",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "GovernmentLevel",
                table: "Schemes");
        }
    }
}
