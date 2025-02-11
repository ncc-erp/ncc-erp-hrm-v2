using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class add_column_BeTViecDate_and_rename_BeStaffDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartWorkingDate",
                table: "Employees",
                newName: "BeTViecDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "BeStaffDate",
                table: "Employees",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeStaffDate",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "BeTViecDate",
                table: "Employees",
                newName: "StartWorkingDate");
        }
    }
}
