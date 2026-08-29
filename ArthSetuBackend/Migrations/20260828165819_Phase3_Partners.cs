using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArthSetuBackend.Migrations
{
    /// <inheritdoc />
    public partial class Phase3_Partners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastVerifiedAt",
                table: "ChannelPartners",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pincode",
                table: "ChannelPartners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SourceSnapshot",
                table: "ChannelPartners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "ChannelPartners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "ChannelPartners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastVerifiedAt",
                table: "ChannelPartners");

            migrationBuilder.DropColumn(
                name: "Pincode",
                table: "ChannelPartners");

            migrationBuilder.DropColumn(
                name: "SourceSnapshot",
                table: "ChannelPartners");

            migrationBuilder.DropColumn(
                name: "State",
                table: "ChannelPartners");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "ChannelPartners");
        }
    }
}
