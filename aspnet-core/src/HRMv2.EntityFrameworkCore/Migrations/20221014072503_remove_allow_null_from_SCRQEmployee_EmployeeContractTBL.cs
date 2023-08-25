using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class remove_allow_null_from_SCRQEmployee_EmployeeContractTBL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeContracts_SalaryChangeRequestEmployees_SalaryReques~",
                table: "EmployeeContracts");

            migrationBuilder.AlterColumn<long>(
                name: "SalaryRequestEmployeeId",
                table: "EmployeeContracts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeContracts_SalaryChangeRequestEmployees_SalaryReques~",
                table: "EmployeeContracts",
                column: "SalaryRequestEmployeeId",
                principalTable: "SalaryChangeRequestEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeContracts_SalaryChangeRequestEmployees_SalaryReques~",
                table: "EmployeeContracts");

            migrationBuilder.AlterColumn<long>(
                name: "SalaryRequestEmployeeId",
                table: "EmployeeContracts",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeContracts_SalaryChangeRequestEmployees_SalaryReques~",
                table: "EmployeeContracts",
                column: "SalaryRequestEmployeeId",
                principalTable: "SalaryChangeRequestEmployees",
                principalColumn: "Id");
        }
    }
}
