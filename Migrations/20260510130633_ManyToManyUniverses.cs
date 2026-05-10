using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SceneBuilderApi.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyUniverses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scenes_Universes_UniverseId",
                table: "Scenes");

            migrationBuilder.DropForeignKey(
                name: "FK_Stories_Universes_UniverseId",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_UniverseId",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Scenes_UniverseId",
                table: "Scenes");

            migrationBuilder.DropColumn(
                name: "UniverseId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "UniverseId",
                table: "Scenes");

            migrationBuilder.CreateTable(
                name: "SceneUniverses",
                columns: table => new
                {
                    SceneId = table.Column<int>(type: "int", nullable: false),
                    UniverseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneUniverses", x => new { x.SceneId, x.UniverseId });
                    table.ForeignKey(
                        name: "FK_SceneUniverses_Scenes_SceneId",
                        column: x => x.SceneId,
                        principalTable: "Scenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SceneUniverses_Universes_UniverseId",
                        column: x => x.UniverseId,
                        principalTable: "Universes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StoryUniverses",
                columns: table => new
                {
                    StoryId = table.Column<int>(type: "int", nullable: false),
                    UniverseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryUniverses", x => new { x.StoryId, x.UniverseId });
                    table.ForeignKey(
                        name: "FK_StoryUniverses_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryUniverses_Universes_UniverseId",
                        column: x => x.UniverseId,
                        principalTable: "Universes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SceneUniverses_UniverseId",
                table: "SceneUniverses",
                column: "UniverseId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryUniverses_UniverseId",
                table: "StoryUniverses",
                column: "UniverseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SceneUniverses");

            migrationBuilder.DropTable(
                name: "StoryUniverses");

            migrationBuilder.AddColumn<int>(
                name: "UniverseId",
                table: "Stories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UniverseId",
                table: "Scenes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stories_UniverseId",
                table: "Stories",
                column: "UniverseId");

            migrationBuilder.CreateIndex(
                name: "IX_Scenes_UniverseId",
                table: "Scenes",
                column: "UniverseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scenes_Universes_UniverseId",
                table: "Scenes",
                column: "UniverseId",
                principalTable: "Universes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_Universes_UniverseId",
                table: "Stories",
                column: "UniverseId",
                principalTable: "Universes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
