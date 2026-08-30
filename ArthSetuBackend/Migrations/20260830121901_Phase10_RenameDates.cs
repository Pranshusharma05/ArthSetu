using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArthSetuBackend.Migrations
{
    /// <inheritdoc />
    public partial class Phase10_RenameDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApplicationStartDate",
                table: "Schemes",
                newName: "ApplicationOpenUntil");

            migrationBuilder.RenameColumn(
                name: "ApplicationEndDate",
                table: "Schemes",
                newName: "ApplicationOpenFrom");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationWindowStatus",
                table: "Schemes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FreshOrRenewal",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationWindowStatus",
                table: "Schemes");

            migrationBuilder.DropColumn(
                name: "FreshOrRenewal",
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

            migrationBuilder.RenameColumn(
                name: "ApplicationOpenUntil",
                table: "Schemes",
                newName: "ApplicationStartDate");

            migrationBuilder.RenameColumn(
                name: "ApplicationOpenFrom",
                table: "Schemes",
                newName: "ApplicationEndDate");
        }
    }
}
