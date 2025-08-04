using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SunNext.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSomeTablesProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CommissioningDate",
                table: "SolarAssets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "SolarAssets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissioningDate",
                table: "SolarAssets");

            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "SolarAssets");
        }
    }
}
