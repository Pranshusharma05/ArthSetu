using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArthSetuBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectorModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "SourceSyncLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConflictsCreated",
                table: "SourceSyncLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Connector",
                table: "SourceSyncLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ContentChanged",
                table: "SourceSyncLogs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FetchStatus",
                table: "SourceSyncLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HttpStatus",
                table: "SourceSyncLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordsDiscovered",
                table: "SourceSyncLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordsImported",
                table: "SourceSyncLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordsNeedingReview",
                table: "SourceSyncLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordsParsed",
                table: "SourceSyncLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordsSkipped",
                table: "SourceSyncLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordsUnchanged",
                table: "SourceSyncLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordsUpdated",
                table: "SourceSyncLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SnapshotId",
                table: "SourceSyncLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "SourceSyncLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConnectorVersion",
                table: "SourceSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentHash",
                table: "SourceSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "SourceSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ETag",
                table: "SourceSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalUrl",
                table: "SourceSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModified",
                table: "SourceSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedDate",
                table: "SourceSnapshots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "SourceSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "SourceSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "SourceSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "GovernmentSources",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExtractedAt",
                table: "FieldProvenance",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawValue",
                table: "FieldProvenance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceLocation",
                table: "FieldProvenance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "FieldProvenance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "FieldProvenance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentStatus",
                table: "EligibilityRuleVersions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                table: "EligibilityRuleVersions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveTo",
                table: "EligibilityRuleVersions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedValue",
                table: "EligibilityRuleVersions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchemeId",
                table: "EligibilityRuleVersions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceEvidence",
                table: "EligibilityRuleVersions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceSnapshotId",
                table: "EligibilityRuleVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupersededBy",
                table: "EligibilityRuleVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "EligibilityRuleVersions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "ConflictsCreated",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "Connector",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "ContentChanged",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "FetchStatus",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "HttpStatus",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "RecordsDiscovered",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "RecordsImported",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "RecordsNeedingReview",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "RecordsParsed",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "RecordsSkipped",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "RecordsUnchanged",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "RecordsUpdated",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "SnapshotId",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "SourceSyncLogs");

            migrationBuilder.DropColumn(
                name: "ConnectorVersion",
                table: "SourceSnapshots");

            migrationBuilder.DropColumn(
                name: "ContentHash",
                table: "SourceSnapshots");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "SourceSnapshots");

            migrationBuilder.DropColumn(
                name: "ETag",
                table: "SourceSnapshots");

            migrationBuilder.DropColumn(
                name: "FinalUrl",
                table: "SourceSnapshots");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "SourceSnapshots");

            migrationBuilder.DropColumn(
                name: "PublishedDate",
                table: "SourceSnapshots");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "SourceSnapshots");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "SourceSnapshots");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "SourceSnapshots");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "GovernmentSources");

            migrationBuilder.DropColumn(
                name: "ExtractedAt",
                table: "FieldProvenance");

            migrationBuilder.DropColumn(
                name: "RawValue",
                table: "FieldProvenance");

            migrationBuilder.DropColumn(
                name: "SourceLocation",
                table: "FieldProvenance");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "FieldProvenance");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "FieldProvenance");

            migrationBuilder.DropColumn(
                name: "CurrentStatus",
                table: "EligibilityRuleVersions");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "EligibilityRuleVersions");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "EligibilityRuleVersions");

            migrationBuilder.DropColumn(
                name: "NormalizedValue",
                table: "EligibilityRuleVersions");

            migrationBuilder.DropColumn(
                name: "SchemeId",
                table: "EligibilityRuleVersions");

            migrationBuilder.DropColumn(
                name: "SourceEvidence",
                table: "EligibilityRuleVersions");

            migrationBuilder.DropColumn(
                name: "SourceSnapshotId",
                table: "EligibilityRuleVersions");

            migrationBuilder.DropColumn(
                name: "SupersededBy",
                table: "EligibilityRuleVersions");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "EligibilityRuleVersions");
        }
    }
}
