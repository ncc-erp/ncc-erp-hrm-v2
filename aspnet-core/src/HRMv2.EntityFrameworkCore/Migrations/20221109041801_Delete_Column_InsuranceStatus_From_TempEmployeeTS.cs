using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class Delete_Column_InsuranceStatus_From_TempEmployeeTS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsuranceStatus",
                table: "TempEmployeeTSs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InsuranceStatus",
                table: "TempEmployeeTSs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
