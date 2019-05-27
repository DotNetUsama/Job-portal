using Microsoft.EntityFrameworkCore.Migrations;

namespace Job_Portal_System.Migrations
{
    public partial class AddGeoDistanceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeoDistances",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Distance = table.Column<long>(nullable: false),
                    City1Id = table.Column<string>(nullable: true),
                    City2Id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoDistances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeoDistances_Cities_City1Id",
                        column: x => x.City1Id,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeoDistances_Cities_City2Id",
                        column: x => x.City2Id,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeoDistances_City1Id",
                table: "GeoDistances",
                column: "City1Id");

            migrationBuilder.CreateIndex(
                name: "IX_GeoDistances_City2Id",
                table: "GeoDistances",
                column: "City2Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeoDistances");
        }
    }
}
