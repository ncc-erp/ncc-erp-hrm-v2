using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class add_bank_info_to_PayslipTBL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "Payslips",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BankId",
                table: "Payslips",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Payslips",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payslips_BankId",
                table: "Payslips",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payslips_Banks_BankId",
                table: "Payslips",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payslips_Banks_BankId",
                table: "Payslips");

            migrationBuilder.DropIndex(
                name: "IX_Payslips_BankId",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Payslips");
        }
    }
}
