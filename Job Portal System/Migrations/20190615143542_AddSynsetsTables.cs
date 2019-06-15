using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Job_Portal_System.Migrations
{
    public partial class AddSynsetsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SkillSynsetId",
                table: "Skills",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "JobTitleSynsetId",
                table: "JobTitles",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FieldOfStudySynsetId",
                table: "FieldOfStudies",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FieldOfStudySynsets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldOfStudySynsets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTitleSynsets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTitleSynsets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillSynsets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillSynsets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SkillSynsetId",
                table: "Skills",
                column: "SkillSynsetId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTitles_JobTitleSynsetId",
                table: "JobTitles",
                column: "JobTitleSynsetId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldOfStudies_FieldOfStudySynsetId",
                table: "FieldOfStudies",
                column: "FieldOfStudySynsetId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldOfStudies_FieldOfStudySynsets_FieldOfStudySynsetId",
                table: "FieldOfStudies",
                column: "FieldOfStudySynsetId",
                principalTable: "FieldOfStudySynsets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobTitles_JobTitleSynsets_JobTitleSynsetId",
                table: "JobTitles",
                column: "JobTitleSynsetId",
                principalTable: "JobTitleSynsets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillSynsets_SkillSynsetId",
                table: "Skills",
                column: "SkillSynsetId",
                principalTable: "SkillSynsets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldOfStudies_FieldOfStudySynsets_FieldOfStudySynsetId",
                table: "FieldOfStudies");

            migrationBuilder.DropForeignKey(
                name: "FK_JobTitles_JobTitleSynsets_JobTitleSynsetId",
                table: "JobTitles");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillSynsets_SkillSynsetId",
                table: "Skills");

            migrationBuilder.DropTable(
                name: "FieldOfStudySynsets");

            migrationBuilder.DropTable(
                name: "JobTitleSynsets");

            migrationBuilder.DropTable(
                name: "SkillSynsets");

            migrationBuilder.DropIndex(
                name: "IX_Skills_SkillSynsetId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_JobTitles_JobTitleSynsetId",
                table: "JobTitles");

            migrationBuilder.DropIndex(
                name: "IX_FieldOfStudies_FieldOfStudySynsetId",
                table: "FieldOfStudies");

            migrationBuilder.DropColumn(
                name: "SkillSynsetId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "JobTitleSynsetId",
                table: "JobTitles");

            migrationBuilder.DropColumn(
                name: "FieldOfStudySynsetId",
                table: "FieldOfStudies");
        }
    }
}
