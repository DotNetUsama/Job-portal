using Microsoft.EntityFrameworkCore.Migrations;

namespace Job_Portal_System.Migrations
{
    public partial class MakeBiographyNullableInResumesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Biography",
                table: "Resumes",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Biography",
                table: "Resumes",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
