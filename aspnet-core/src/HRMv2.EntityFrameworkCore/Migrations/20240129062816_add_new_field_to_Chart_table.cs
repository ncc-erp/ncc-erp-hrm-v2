using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class add_new_field_to_Chart_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShareToUserIds",
                table: "Charts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shareToRoleIds",
                table: "Charts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShareToUserIds",
                table: "Charts");

            migrationBuilder.DropColumn(
                name: "shareToRoleIds",
                table: "Charts");
        }
    }
}
