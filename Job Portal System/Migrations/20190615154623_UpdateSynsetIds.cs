using Microsoft.EntityFrameworkCore.Migrations;

namespace Job_Portal_System.Migrations
{
    public partial class UpdateSynsetIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<long>(
                name: "SkillSynsetId",
                table: "Skills",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "JobTitleSynsetId",
                table: "JobTitles",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "FieldOfStudySynsetId",
                table: "FieldOfStudies",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldOfStudies_FieldOfStudySynsets_FieldOfStudySynsetId",
                table: "FieldOfStudies",
                column: "FieldOfStudySynsetId",
                principalTable: "FieldOfStudySynsets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobTitles_JobTitleSynsets_JobTitleSynsetId",
                table: "JobTitles",
                column: "JobTitleSynsetId",
                principalTable: "JobTitleSynsets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillSynsets_SkillSynsetId",
                table: "Skills",
                column: "SkillSynsetId",
                principalTable: "SkillSynsets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.AlterColumn<long>(
                name: "SkillSynsetId",
                table: "Skills",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "JobTitleSynsetId",
                table: "JobTitles",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "FieldOfStudySynsetId",
                table: "FieldOfStudies",
                nullable: true,
                oldClrType: typeof(long));

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
    }
}
