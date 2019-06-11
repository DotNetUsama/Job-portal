using Microsoft.EntityFrameworkCore.Migrations;

namespace Job_Portal_System.Migrations
{
    public partial class UpdateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schools_Cities_CityId",
                table: "Schools");

            migrationBuilder.DropTable(
                name: "FieldOfStudySimilarities");

            migrationBuilder.DropTable(
                name: "JobTitleSimilarities");

            migrationBuilder.DropTable(
                name: "SimilarFieldOfStudyTitles");

            migrationBuilder.DropTable(
                name: "SimilarJobTitles");

            migrationBuilder.DropIndex(
                name: "IX_Schools_CityId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "IsSeeking",
                table: "JobSeekers");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "WorkExperiences",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Skills",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedTitle",
                table: "Skills",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Schools",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Schools",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "Resumes",
                type: "text",
                nullable: true);

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

            migrationBuilder.AddColumn<long>(
                name: "DistanceLimit",
                table: "JobVacancies",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedTitle",
                table: "JobTitles",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedTitle",
                table: "FieldOfStudies",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Companies",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FoundedYear",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Companies",
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Announcements",
                type: "text",
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedTitle",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "Biography",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "IsSeeking",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "MovingDistanceLimit",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "DistanceLimit",
                table: "JobVacancies");

            migrationBuilder.DropColumn(
                name: "NormalizedTitle",
                table: "JobTitles");

            migrationBuilder.DropColumn(
                name: "NormalizedTitle",
                table: "FieldOfStudies");

            migrationBuilder.DropColumn(
                name: "FoundedYear",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "WorkExperiences",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Skills",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Schools",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(
                name: "CityId",
                table: "Schools",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSeeking",
                table: "JobSeekers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Companies",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Announcements",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "SimilarFieldOfStudyTitles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimilarFieldOfStudyTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimilarJobTitles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimilarJobTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldOfStudySimilarities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    FieldOfStudyId = table.Column<long>(nullable: true),
                    SimilarTitleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldOfStudySimilarities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldOfStudySimilarities_FieldOfStudies_FieldOfStudyId",
                        column: x => x.FieldOfStudyId,
                        principalTable: "FieldOfStudies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FieldOfStudySimilarities_SimilarFieldOfStudyTitles_SimilarTitleId",
                        column: x => x.SimilarTitleId,
                        principalTable: "SimilarFieldOfStudyTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobTitleSimilarities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    JobTitleId = table.Column<long>(nullable: false),
                    SimilarTitleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTitleSimilarities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobTitleSimilarities_JobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobTitleSimilarities_SimilarJobTitles_SimilarTitleId",
                        column: x => x.SimilarTitleId,
                        principalTable: "SimilarJobTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schools_CityId",
                table: "Schools",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldOfStudySimilarities_FieldOfStudyId",
                table: "FieldOfStudySimilarities",
                column: "FieldOfStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldOfStudySimilarities_SimilarTitleId",
                table: "FieldOfStudySimilarities",
                column: "SimilarTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTitleSimilarities_JobTitleId",
                table: "JobTitleSimilarities",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTitleSimilarities_SimilarTitleId",
                table: "JobTitleSimilarities",
                column: "SimilarTitleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_Cities_CityId",
                table: "Schools",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
