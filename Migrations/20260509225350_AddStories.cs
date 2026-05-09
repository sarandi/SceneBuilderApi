using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SceneBuilderApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create Stories table
            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stories_UserId",
                table: "Stories",
                column: "UserId");

            // 2. Seed a default story for each existing user
            migrationBuilder.Sql(@"
                INSERT INTO Stories (UserId, Title, CreatedAt, UpdatedAt)
                SELECT Id, 'My Story', GETUTCDATE(), GETUTCDATE()
                FROM Users;
            ");

            // 3. Add StoryId to Scenes as nullable (no FK yet)
            migrationBuilder.AddColumn<int>(
                name: "StoryId",
                table: "Scenes",
                type: "int",
                nullable: true);

            // 4. Point each scene to the default story for its user
            migrationBuilder.Sql(@"
                UPDATE s
                SET s.StoryId = st.Id
                FROM Scenes s
                INNER JOIN Stories st ON s.UserId = st.UserId;
            ");

            // 5. Make StoryId NOT NULL now that all rows are populated
            migrationBuilder.Sql("ALTER TABLE Scenes ALTER COLUMN StoryId int NOT NULL;");

            // 6. Drop old UserId FK, index, and column
            migrationBuilder.DropForeignKey(
                name: "FK_Scenes_Users_UserId",
                table: "Scenes");

            migrationBuilder.DropIndex(
                name: "IX_Scenes_UserId",
                table: "Scenes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Scenes");

            // 7. Add StoryId index and FK
            migrationBuilder.CreateIndex(
                name: "IX_Scenes_StoryId",
                table: "Scenes",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scenes_Stories_StoryId",
                table: "Scenes",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scenes_Stories_StoryId",
                table: "Scenes");

            migrationBuilder.DropTable(
                name: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Scenes_StoryId",
                table: "Scenes");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "Scenes");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Scenes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Scenes_UserId",
                table: "Scenes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scenes_Users_UserId",
                table: "Scenes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
