using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArthSetuBackend.Migrations
{
    /// <inheritdoc />
    public partial class Phase1_DiscoveryRegistry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscoveryCandidates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExternalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscoverySource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GovernmentLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CandidateMinistry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CandidateDepartment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CandidateStateUT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CandidateOwningAuthority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CandidateApplicationPortal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscoveredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolutionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscoveryCandidates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscoveryCandidates");
        }
    }
}
