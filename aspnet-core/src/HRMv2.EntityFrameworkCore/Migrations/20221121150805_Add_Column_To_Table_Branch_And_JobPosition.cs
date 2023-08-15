using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class Add_Column_To_Table_Branch_And_JobPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameInContract",
                table: "JobPositions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CEOId",
                table: "Branches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "CompanyPhone",
                table: "Branches",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyTaxCode",
                table: "Branches",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameInContract",
                table: "Branches",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameInContract",
                table: "JobPositions");

            migrationBuilder.DropColumn(
                name: "CEOId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "CompanyPhone",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "CompanyTaxCode",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "NameInContract",
                table: "Branches");
        }
    }
}
