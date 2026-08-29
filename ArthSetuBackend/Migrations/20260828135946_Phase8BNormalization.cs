using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArthSetuBackend.Migrations
{
    /// <inheritdoc />
    public partial class Phase8BNormalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchemeBenefitComponentId",
                table: "SourceConflicts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupersededBy",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SchemeBenefitComponentId",
                table: "SchemeEligibilityRules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SchemeBenefitComponentId",
                table: "EligibilityRuleVersions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchemeEligibilityRules_SchemeBenefitComponentId",
                table: "SchemeEligibilityRules",
                column: "SchemeBenefitComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemeEligibilityRules_SchemeBenefitComponents_SchemeBenefitComponentId",
                table: "SchemeEligibilityRules",
                column: "SchemeBenefitComponentId",
                principalTable: "SchemeBenefitComponents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemeEligibilityRules_SchemeBenefitComponents_SchemeBenefitComponentId",
                table: "SchemeEligibilityRules");

            migrationBuilder.DropIndex(
                name: "IX_SchemeEligibilityRules_SchemeBenefitComponentId",
                table: "SchemeEligibilityRules");

            migrationBuilder.DropColumn(
                name: "SchemeBenefitComponentId",
                table: "SourceConflicts");

            migrationBuilder.DropColumn(
                name: "SupersededBy",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "SchemeBenefitComponentId",
                table: "SchemeEligibilityRules");

            migrationBuilder.DropColumn(
                name: "SchemeBenefitComponentId",
                table: "EligibilityRuleVersions");
        }
    }
}
