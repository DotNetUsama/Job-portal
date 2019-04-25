using Microsoft.EntityFrameworkCore.Migrations;

namespace Job_Portal_System.Migrations
{
    public partial class AddCompanyDepartmentColumnToJobVacanciesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobVacancies_Cities_CityId",
                table: "JobVacancies");

            migrationBuilder.RenameColumn(
                name: "CityId",
                table: "JobVacancies",
                newName: "CompanyDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_JobVacancies_CityId",
                table: "JobVacancies",
                newName: "IX_JobVacancies_CompanyDepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobVacancies_CompanyDepartments_CompanyDepartmentId",
                table: "JobVacancies",
                column: "CompanyDepartmentId",
                principalTable: "CompanyDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobVacancies_CompanyDepartments_CompanyDepartmentId",
                table: "JobVacancies");

            migrationBuilder.RenameColumn(
                name: "CompanyDepartmentId",
                table: "JobVacancies",
                newName: "CityId");

            migrationBuilder.RenameIndex(
                name: "IX_JobVacancies_CompanyDepartmentId",
                table: "JobVacancies",
                newName: "IX_JobVacancies_CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobVacancies_Cities_CityId",
                table: "JobVacancies",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
