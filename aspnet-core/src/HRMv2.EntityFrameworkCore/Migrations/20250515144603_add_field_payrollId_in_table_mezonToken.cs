using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class add_field_payrollId_in_table_mezonToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PayrollId",
                table: "MezonTokens",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayrollId",
                table: "MezonTokens");
        }
    }
}
