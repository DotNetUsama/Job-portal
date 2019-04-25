using Microsoft.EntityFrameworkCore.Migrations;

namespace Job_Portal_System.Migrations
{
    public partial class CompanyIdInCompanyDepartmentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyDepartments_Companies_CompanyId1",
                table: "CompanyDepartments");

            migrationBuilder.DropIndex(
                name: "IX_CompanyDepartments_CompanyId1",
                table: "CompanyDepartments");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "CompanyDepartments");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "CompanyDepartments",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDepartments_CompanyId",
                table: "CompanyDepartments",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyDepartments_Companies_CompanyId",
                table: "CompanyDepartments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyDepartments_Companies_CompanyId",
                table: "CompanyDepartments");

            migrationBuilder.DropIndex(
                name: "IX_CompanyDepartments_CompanyId",
                table: "CompanyDepartments");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                table: "CompanyDepartments",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyId1",
                table: "CompanyDepartments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDepartments_CompanyId1",
                table: "CompanyDepartments",
                column: "CompanyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyDepartments_Companies_CompanyId1",
                table: "CompanyDepartments",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
