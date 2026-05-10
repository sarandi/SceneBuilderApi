using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SceneBuilderApi.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitySystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UniverseId",
                table: "Stories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBuiltIn = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityTypes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelationshipTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsBuiltIn = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationshipTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelationshipTypes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Universes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Universes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityTypeFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityTypeId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RefEntityTypeId = table.Column<int>(type: "int", nullable: true),
                    IsBuiltIn = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsGmOnly = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypeFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityTypeFields_EntityTypes_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityTypeFields_EntityTypes_RefEntityTypeId",
                        column: x => x.RefEntityTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Calendars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UniverseId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseUnitName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BaseUnitDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseUnitInIntegers = table.Column<int>(type: "int", nullable: false),
                    CurrentDay = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calendars_Universes_UniverseId",
                        column: x => x.UniverseId,
                        principalTable: "Universes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Calendars_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UniverseId = table.Column<int>(type: "int", nullable: true),
                    EntityTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    IsSecret = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entities_EntityTypes_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entities_Universes_UniverseId",
                        column: x => x.UniverseId,
                        principalTable: "Universes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Entities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CalendarEras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalendarId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDay = table.Column<int>(type: "int", nullable: false),
                    EndDay = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarEras_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalendarSpecialDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalendarId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    DayOfYear = table.Column<int>(type: "int", nullable: true),
                    AbsoluteDay = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Significance = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarSpecialDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarSpecialDates_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalendarUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalendarId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PluralName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentUnitId = table.Column<int>(type: "int", nullable: true),
                    Direction = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CountPerParent = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarUnits_CalendarUnits_ParentUnitId",
                        column: x => x.ParentUnitId,
                        principalTable: "CalendarUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarUnits_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityFieldRefValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    RefEntityId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsSecret = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityFieldRefValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityFieldRefValues_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityFieldRefValues_Entities_RefEntityId",
                        column: x => x.RefEntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityFieldRefValues_EntityTypeFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "EntityTypeFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntityFieldValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberValue = table.Column<double>(type: "float", nullable: true),
                    BoolValue = table.Column<bool>(type: "bit", nullable: true),
                    IsSecret = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityFieldValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityFieldValues_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityFieldValues_EntityTypeFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "EntityTypeFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntityRelationships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromEntityId = table.Column<int>(type: "int", nullable: false),
                    ToEntityId = table.Column<int>(type: "int", nullable: false),
                    RelationshipTypeId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSecret = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityRelationships_Entities_FromEntityId",
                        column: x => x.FromEntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityRelationships_Entities_ToEntityId",
                        column: x => x.ToEntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityRelationships_RelationshipTypes_RelationshipTypeId",
                        column: x => x.RelationshipTypeId,
                        principalTable: "RelationshipTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stories_UniverseId",
                table: "Stories",
                column: "UniverseId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEras_CalendarId",
                table: "CalendarEras",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_Calendars_UniverseId",
                table: "Calendars",
                column: "UniverseId");

            migrationBuilder.CreateIndex(
                name: "IX_Calendars_UserId",
                table: "Calendars",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSpecialDates_CalendarId",
                table: "CalendarSpecialDates",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarUnits_CalendarId",
                table: "CalendarUnits",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarUnits_ParentUnitId",
                table: "CalendarUnits",
                column: "ParentUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_EntityTypeId",
                table: "Entities",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_UniverseId",
                table: "Entities",
                column: "UniverseId");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_UserId",
                table: "Entities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFieldRefValues_EntityId",
                table: "EntityFieldRefValues",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFieldRefValues_FieldId",
                table: "EntityFieldRefValues",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFieldRefValues_RefEntityId",
                table: "EntityFieldRefValues",
                column: "RefEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFieldValues_EntityId",
                table: "EntityFieldValues",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFieldValues_FieldId",
                table: "EntityFieldValues",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRelationships_FromEntityId",
                table: "EntityRelationships",
                column: "FromEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRelationships_RelationshipTypeId",
                table: "EntityRelationships",
                column: "RelationshipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRelationships_ToEntityId",
                table: "EntityRelationships",
                column: "ToEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypeFields_EntityTypeId",
                table: "EntityTypeFields",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypeFields_RefEntityTypeId",
                table: "EntityTypeFields",
                column: "RefEntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypes_UserId",
                table: "EntityTypes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RelationshipTypes_UserId",
                table: "RelationshipTypes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Universes_UserId",
                table: "Universes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_Universes_UniverseId",
                table: "Stories",
                column: "UniverseId",
                principalTable: "Universes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_Universes_UniverseId",
                table: "Stories");

            migrationBuilder.DropTable(
                name: "CalendarEras");

            migrationBuilder.DropTable(
                name: "CalendarSpecialDates");

            migrationBuilder.DropTable(
                name: "CalendarUnits");

            migrationBuilder.DropTable(
                name: "EntityFieldRefValues");

            migrationBuilder.DropTable(
                name: "EntityFieldValues");

            migrationBuilder.DropTable(
                name: "EntityRelationships");

            migrationBuilder.DropTable(
                name: "Calendars");

            migrationBuilder.DropTable(
                name: "EntityTypeFields");

            migrationBuilder.DropTable(
                name: "Entities");

            migrationBuilder.DropTable(
                name: "RelationshipTypes");

            migrationBuilder.DropTable(
                name: "EntityTypes");

            migrationBuilder.DropTable(
                name: "Universes");

            migrationBuilder.DropIndex(
                name: "IX_Stories_UniverseId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "UniverseId",
                table: "Stories");
        }
    }
}
