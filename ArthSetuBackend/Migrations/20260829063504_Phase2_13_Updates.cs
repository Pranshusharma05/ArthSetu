using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArthSetuBackend.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_13_Updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FreshnessStatus",
                table: "GovernmentSources",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAttemptedSyncAt",
                table: "GovernmentSources",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastError",
                table: "GovernmentSources",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SourceUpdatedAt",
                table: "GovernmentSources",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SchemeSourceReferences",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SchemeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExternalReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeSourceReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemeSourceReferences_GovernmentSources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "GovernmentSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchemeSourceReferences_Schemes_SchemeId",
                        column: x => x.SchemeId,
                        principalTable: "Schemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchemeSourceReferences_SchemeId",
                table: "SchemeSourceReferences",
                column: "SchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeSourceReferences_SourceId",
                table: "SchemeSourceReferences",
                column: "SourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchemeSourceReferences");

            migrationBuilder.DropColumn(
                name: "FreshnessStatus",
                table: "GovernmentSources");

            migrationBuilder.DropColumn(
                name: "LastAttemptedSyncAt",
                table: "GovernmentSources");

            migrationBuilder.DropColumn(
                name: "LastError",
                table: "GovernmentSources");

            migrationBuilder.DropColumn(
                name: "SourceUpdatedAt",
                table: "GovernmentSources");
        }
    }
}
