using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class add_column_userMezonId_in_table_user_and_employee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserMezonId",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserMezonId",
                table: "AbpUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MezonTokens_PayrollId",
                table: "MezonTokens",
                column: "PayrollId");

            migrationBuilder.AddForeignKey(
                name: "FK_MezonTokens_Payrolls_PayrollId",
                table: "MezonTokens",
                column: "PayrollId",
                principalTable: "Payrolls",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MezonTokens_Payrolls_PayrollId",
                table: "MezonTokens");

            migrationBuilder.DropIndex(
                name: "IX_MezonTokens_PayrollId",
                table: "MezonTokens");

            migrationBuilder.DropColumn(
                name: "UserMezonId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UserMezonId",
                table: "AbpUsers");
        }
    }
}
