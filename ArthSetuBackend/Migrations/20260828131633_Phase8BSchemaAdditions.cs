using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArthSetuBackend.Migrations
{
    /// <inheritdoc />
    public partial class Phase8BSchemaAdditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicationEndDate",
                table: "Schemes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationPortal",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicationStartDate",
                table: "Schemes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscoveryPortal",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LifecycleStatus",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialRuleSource",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwningAuthority",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SchemeApplicationWindows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchemeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Cycle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceSnapshotId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeApplicationWindows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemeApplicationWindows_Schemes_SchemeId",
                        column: x => x.SchemeId,
                        principalTable: "Schemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchemeBenefitComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchemeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BenefitType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComponentDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeBenefitComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemeBenefitComponents_Schemes_SchemeId",
                        column: x => x.SchemeId,
                        principalTable: "Schemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchemeApplicationWindows_SchemeId",
                table: "SchemeApplicationWindows",
                column: "SchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeBenefitComponents_SchemeId",
                table: "SchemeBenefitComponents",
                column: "SchemeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchemeApplicationWindows");

            migrationBuilder.DropTable(
                name: "SchemeBenefitComponents");

            migrationBuilder.DropColumn(
                name: "ApplicationEndDate",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "ApplicationPortal",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "ApplicationStartDate",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "DiscoveryPortal",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "LifecycleStatus",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "OfficialRuleSource",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "OwningAuthority",
                table: "Schemes");
        }
    }
}
