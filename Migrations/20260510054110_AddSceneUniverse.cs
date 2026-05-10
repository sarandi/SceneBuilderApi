using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SceneBuilderApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSceneUniverse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UniverseId",
                table: "Scenes",
                type: "int",
                nullable: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scenes_Universes_UniverseId",
                table: "Scenes");

            migrationBuilder.DropIndex(
                name: "IX_Scenes_UniverseId",
                table: "Scenes");

            migrationBuilder.DropColumn(
                name: "UniverseId",
                table: "Scenes");
        }
    }
}
