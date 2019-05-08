using Microsoft.EntityFrameworkCore.Migrations;

namespace Job_Portal_System.Migrations
{
    public partial class UpdateJobVacanciesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeciderFile",
                table: "JobVacancies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeciderFile",
                table: "JobVacancies",
                maxLength: 64,
                nullable: true);
        }
    }
}
