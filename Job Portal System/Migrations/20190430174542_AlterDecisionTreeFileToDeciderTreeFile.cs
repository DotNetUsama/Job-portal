using Microsoft.EntityFrameworkCore.Migrations;

namespace Job_Portal_System.Migrations
{
    public partial class AlterDecisionTreeFileToDeciderTreeFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DecisionTreeFile",
                table: "JobVacancies",
                newName: "DeciderFile");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeciderFile",
                table: "JobVacancies",
                newName: "DecisionTreeFile");
        }
    }
}
