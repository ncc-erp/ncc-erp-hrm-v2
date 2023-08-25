using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class create_TempEmployeeTalentTBL_and_add_PersonalEmail_to_employee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PersonalEmail",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TempEmployeeTalents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    NCCEmail = table.Column<string>(type: "text", nullable: true),
                    PersonalEmail = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    UserType = table.Column<int>(type: "integer", nullable: true),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    JobPositionId = table.Column<long>(type: "bigint", nullable: true),
                    LevelId = table.Column<long>(type: "bigint", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OnboardDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    Salary = table.Column<double>(type: "double precision", nullable: true),
                    ProbationPercentage = table.Column<int>(type: "integer", nullable: true),
                    Skills = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_TempEmployeeTalents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TempEmployeeTalents_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TempEmployeeTalents_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TempEmployeeTalents_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TempEmployeeTalents_JobPositions_JobPositionId",
                        column: x => x.JobPositionId,
                        principalTable: "JobPositions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TempEmployeeTalents_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TempEmployeeTalents_BranchId",
                table: "TempEmployeeTalents",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TempEmployeeTalents_CreatorUserId",
                table: "TempEmployeeTalents",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TempEmployeeTalents_JobPositionId",
                table: "TempEmployeeTalents",
                column: "JobPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_TempEmployeeTalents_LastModifierUserId",
                table: "TempEmployeeTalents",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TempEmployeeTalents_LevelId",
                table: "TempEmployeeTalents",
                column: "LevelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempEmployeeTalents");

            migrationBuilder.DropColumn(
                name: "PersonalEmail",
                table: "Employees");
        }
    }
}
