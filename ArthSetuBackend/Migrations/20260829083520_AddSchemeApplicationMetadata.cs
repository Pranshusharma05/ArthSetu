using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArthSetuBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddSchemeApplicationMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationInstructions",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationMode",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ChannelPartnerRequired",
                table: "Schemes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InstitutionRequired",
                table: "Schemes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LoginRequired",
                table: "Schemes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OfflineAllowed",
                table: "Schemes",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationInstructions",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "ApplicationMode",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "ChannelPartnerRequired",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "InstitutionRequired",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "LoginRequired",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "OfflineAllowed",
                table: "Schemes");
        }
    }
}
