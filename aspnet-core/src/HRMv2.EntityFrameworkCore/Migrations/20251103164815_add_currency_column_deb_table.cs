using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class add_currency_column_deb_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Debts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Debts");
        }
    }
}
