using Microsoft.EntityFrameworkCore.Migrations;

namespace Job_Portal_System.Migrations
{
    public partial class UpdateDatabaseTables1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSeeking",
                table: "JobSeekers");

            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "Resumes",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsSeeking",
                table: "Resumes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "MovingDistanceLimit",
                table: "Resumes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Cities",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Cities",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Biography",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "IsSeeking",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "MovingDistanceLimit",
                table: "Resumes");

            migrationBuilder.AddColumn<bool>(
                name: "IsSeeking",
                table: "JobSeekers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Cities",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Cities",
                nullable: true,
                oldClrType: typeof(double));
        }
    }
}
