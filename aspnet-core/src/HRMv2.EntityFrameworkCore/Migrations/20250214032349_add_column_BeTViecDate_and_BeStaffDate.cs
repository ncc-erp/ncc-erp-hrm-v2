using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class add_column_BeTViecDate_and_BeStaffDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BeStaffDate",
                table: "Employees",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BeTViecDate",
                table: "Employees",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeStaffDate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BeTViecDate",
                table: "Employees");
        }
    }
}
