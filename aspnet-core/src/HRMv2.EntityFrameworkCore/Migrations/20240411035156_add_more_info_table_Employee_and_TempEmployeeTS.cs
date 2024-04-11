using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class add_more_info_table_Employee_and_TempEmployeeTS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentAddress",
                table: "TempEmployeeTSs",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "TempEmployeeTSs",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactPhone",
                table: "TempEmployeeTSs",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentAddress",
                table: "Employees",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "Employees",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactPhone",
                table: "Employees",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBranchHistories_EmployeeId",
                table: "EmployeeBranchHistories",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeBranchHistories_Employees_EmployeeId",
                table: "EmployeeBranchHistories",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeBranchHistories_Employees_EmployeeId",
                table: "EmployeeBranchHistories");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeBranchHistories_EmployeeId",
                table: "EmployeeBranchHistories");

            migrationBuilder.DropColumn(
                name: "CurrentAddress",
                table: "TempEmployeeTSs");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "TempEmployeeTSs");

            migrationBuilder.DropColumn(
                name: "EmergencyContactPhone",
                table: "TempEmployeeTSs");

            migrationBuilder.DropColumn(
                name: "CurrentAddress",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmergencyContactPhone",
                table: "Employees");
        }
    }
}
