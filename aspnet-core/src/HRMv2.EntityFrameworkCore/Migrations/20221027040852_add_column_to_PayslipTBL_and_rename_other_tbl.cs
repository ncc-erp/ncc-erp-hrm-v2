using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMv2.Migrations
{
    public partial class add_column_to_PayslipTBL_and_rename_other_tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branchs_AbpUsers_CreatorUserId",
                table: "Branchs");

            migrationBuilder.DropForeignKey(
                name: "FK_Branchs_AbpUsers_LastModifierUserId",
                table: "Branchs");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeBranchHistories_Branchs_BranchId",
                table: "EmployeeBranchHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Branchs_BranchId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_PayslipSalarys_AbpUsers_CreatorUserId",
                table: "PayslipSalarys");

            migrationBuilder.DropForeignKey(
                name: "FK_PayslipSalarys_AbpUsers_LastModifierUserId",
                table: "PayslipSalarys");

            migrationBuilder.DropForeignKey(
                name: "FK_PayslipSalarys_Payslips_PayslipId",
                table: "PayslipSalarys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayslipSalarys",
                table: "PayslipSalarys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Branchs",
                table: "Branchs");

            migrationBuilder.RenameTable(
                name: "PayslipSalarys",
                newName: "InputPayslipSalaries");

            migrationBuilder.RenameTable(
                name: "Branchs",
                newName: "Branches");

            migrationBuilder.RenameIndex(
                name: "IX_PayslipSalarys_PayslipId",
                table: "InputPayslipSalaries",
                newName: "IX_InputPayslipSalaries_PayslipId");

            migrationBuilder.RenameIndex(
                name: "IX_PayslipSalarys_LastModifierUserId",
                table: "InputPayslipSalaries",
                newName: "IX_InputPayslipSalaries_LastModifierUserId");

            migrationBuilder.RenameIndex(
                name: "IX_PayslipSalarys_CreatorUserId",
                table: "InputPayslipSalaries",
                newName: "IX_InputPayslipSalaries_CreatorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Branchs_LastModifierUserId",
                table: "Branches",
                newName: "IX_Branches_LastModifierUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Branchs_CreatorUserId",
                table: "Branches",
                newName: "IX_Branches_CreatorUserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ComplainDeadline",
                table: "Payslips",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComplainNote",
                table: "Payslips",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConfirmStatus",
                table: "Payslips",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InputPayslipSalaries",
                table: "InputPayslipSalaries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Branches",
                table: "Branches",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_AbpUsers_CreatorUserId",
                table: "Branches",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_AbpUsers_LastModifierUserId",
                table: "Branches",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeBranchHistories_Branches_BranchId",
                table: "EmployeeBranchHistories",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Branches_BranchId",
                table: "Employees",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InputPayslipSalaries_AbpUsers_CreatorUserId",
                table: "InputPayslipSalaries",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InputPayslipSalaries_AbpUsers_LastModifierUserId",
                table: "InputPayslipSalaries",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InputPayslipSalaries_Payslips_PayslipId",
                table: "InputPayslipSalaries",
                column: "PayslipId",
                principalTable: "Payslips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_AbpUsers_CreatorUserId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_AbpUsers_LastModifierUserId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeBranchHistories_Branches_BranchId",
                table: "EmployeeBranchHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Branches_BranchId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_InputPayslipSalaries_AbpUsers_CreatorUserId",
                table: "InputPayslipSalaries");

            migrationBuilder.DropForeignKey(
                name: "FK_InputPayslipSalaries_AbpUsers_LastModifierUserId",
                table: "InputPayslipSalaries");

            migrationBuilder.DropForeignKey(
                name: "FK_InputPayslipSalaries_Payslips_PayslipId",
                table: "InputPayslipSalaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InputPayslipSalaries",
                table: "InputPayslipSalaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Branches",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "ComplainDeadline",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "ComplainNote",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "ConfirmStatus",
                table: "Payslips");

            migrationBuilder.RenameTable(
                name: "InputPayslipSalaries",
                newName: "PayslipSalarys");

            migrationBuilder.RenameTable(
                name: "Branches",
                newName: "Branchs");

            migrationBuilder.RenameIndex(
                name: "IX_InputPayslipSalaries_PayslipId",
                table: "PayslipSalarys",
                newName: "IX_PayslipSalarys_PayslipId");

            migrationBuilder.RenameIndex(
                name: "IX_InputPayslipSalaries_LastModifierUserId",
                table: "PayslipSalarys",
                newName: "IX_PayslipSalarys_LastModifierUserId");

            migrationBuilder.RenameIndex(
                name: "IX_InputPayslipSalaries_CreatorUserId",
                table: "PayslipSalarys",
                newName: "IX_PayslipSalarys_CreatorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Branches_LastModifierUserId",
                table: "Branchs",
                newName: "IX_Branchs_LastModifierUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Branches_CreatorUserId",
                table: "Branchs",
                newName: "IX_Branchs_CreatorUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayslipSalarys",
                table: "PayslipSalarys",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Branchs",
                table: "Branchs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branchs_AbpUsers_CreatorUserId",
                table: "Branchs",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branchs_AbpUsers_LastModifierUserId",
                table: "Branchs",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeBranchHistories_Branchs_BranchId",
                table: "EmployeeBranchHistories",
                column: "BranchId",
                principalTable: "Branchs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Branchs_BranchId",
                table: "Employees",
                column: "BranchId",
                principalTable: "Branchs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayslipSalarys_AbpUsers_CreatorUserId",
                table: "PayslipSalarys",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PayslipSalarys_AbpUsers_LastModifierUserId",
                table: "PayslipSalarys",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PayslipSalarys_Payslips_PayslipId",
                table: "PayslipSalarys",
                column: "PayslipId",
                principalTable: "Payslips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
