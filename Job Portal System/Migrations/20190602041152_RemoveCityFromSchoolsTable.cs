using Microsoft.EntityFrameworkCore.Migrations;

namespace Job_Portal_System.Migrations
{
    public partial class RemoveCityFromSchoolsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schools_Cities_CityId",
                table: "Schools");

            migrationBuilder.DropIndex(
                name: "IX_Schools_CityId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Schools");

            migrationBuilder.AddColumn<long>(
                name: "DistanceLimit",
                table: "JobVacancies",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistanceLimit",
                table: "JobVacancies");

            migrationBuilder.AddColumn<string>(
                name: "CityId",
                table: "Schools",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schools_CityId",
                table: "Schools",
                column: "CityId");

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
