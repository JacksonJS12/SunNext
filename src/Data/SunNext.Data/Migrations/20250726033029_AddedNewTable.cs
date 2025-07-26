using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SunNext.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolarAssets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PowerKw = table.Column<double>(type: "float", nullable: false),
                    CapacityKw = table.Column<double>(type: "float", nullable: false),
                    EfficiencyPercent = table.Column<double>(type: "float", nullable: false),
                    EnergyTodayKWh = table.Column<double>(type: "float", nullable: false),
                    EnergyMonthKWh = table.Column<double>(type: "float", nullable: false),
                    EnergyYearKWh = table.Column<double>(type: "float", nullable: false),
                    EnergyTotalKWh = table.Column<double>(type: "float", nullable: false),
                    InstallerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InstallerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InstallerPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarAssets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolarAssets_IsDeleted",
                table: "SolarAssets",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolarAssets");
        }
    }
}
