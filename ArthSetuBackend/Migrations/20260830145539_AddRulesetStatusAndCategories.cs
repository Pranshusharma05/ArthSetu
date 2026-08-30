using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArthSetuBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddRulesetStatusAndCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RulesetStatus",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConditionField",
                table: "SchemeEligibilityRules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConditionOperator",
                table: "SchemeEligibilityRules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConditionValue",
                table: "SchemeEligibilityRules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EligibilitySourceType",
                table: "SchemeEligibilityRules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernmentSourceId",
                table: "SchemeEligibilityRules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVerifiedAt",
                table: "SchemeEligibilityRules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialSourceUrl",
                table: "SchemeEligibilityRules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceSnapshotId",
                table: "SchemeEligibilityRules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataOrigin",
                table: "LocationMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "LocationMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SchemeDiscoveryCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchemeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeDiscoveryCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemeDiscoveryCategories_Schemes_SchemeId",
                        column: x => x.SchemeId,
                        principalTable: "Schemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchemeDiscoveryCategories_SchemeId",
                table: "SchemeDiscoveryCategories",
                column: "SchemeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchemeDiscoveryCategories");

            migrationBuilder.DropColumn(
                name: "RulesetStatus",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "ConditionField",
                table: "SchemeEligibilityRules");

            migrationBuilder.DropColumn(
                name: "ConditionOperator",
                table: "SchemeEligibilityRules");

            migrationBuilder.DropColumn(
                name: "ConditionValue",
                table: "SchemeEligibilityRules");

            migrationBuilder.DropColumn(
                name: "EligibilitySourceType",
                table: "SchemeEligibilityRules");

            migrationBuilder.DropColumn(
                name: "GovernmentSourceId",
                table: "SchemeEligibilityRules");

            migrationBuilder.DropColumn(
                name: "LastVerifiedAt",
                table: "SchemeEligibilityRules");

            migrationBuilder.DropColumn(
                name: "OfficialSourceUrl",
                table: "SchemeEligibilityRules");

            migrationBuilder.DropColumn(
                name: "SourceSnapshotId",
                table: "SchemeEligibilityRules");

            migrationBuilder.DropColumn(
                name: "DataOrigin",
                table: "LocationMaster");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "LocationMaster");
        }
    }
}
