using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class Create_Table_TempEmployeeTS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TempEmployeeTSs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    RequestStatus = table.Column<int>(type: "integer", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Birthday = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IdCard = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BankId = table.Column<long>(type: "bigint", nullable: true),
                    BankAccountNumber = table.Column<string>(type: "text", nullable: true),
                    IssuedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IssuedBy = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PlaceOfPermanent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    InsuranceStatus = table.Column<int>(type: "integer", nullable: false),
                    TaxCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempEmployeeTSs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TempEmployeeTSs_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TempEmployeeTSs_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TempEmployeeTSs_CreatorUserId",
                table: "TempEmployeeTSs",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TempEmployeeTSs_LastModifierUserId",
                table: "TempEmployeeTSs",
                column: "LastModifierUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempEmployeeTSs");
        }
    }
}
