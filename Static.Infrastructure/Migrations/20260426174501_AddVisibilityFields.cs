using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Static.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVisibilityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "WbsDocument",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTeacherContent",
                table: "WbsDocument",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "UmlDiagram",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTeacherContent",
                table: "UmlDiagram",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "SrsDocument",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTeacherContent",
                table: "SrsDocument",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "WbsDocument");

            migrationBuilder.DropColumn(
                name: "IsTeacherContent",
                table: "WbsDocument");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "UmlDiagram");

            migrationBuilder.DropColumn(
                name: "IsTeacherContent",
                table: "UmlDiagram");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "SrsDocument");

            migrationBuilder.DropColumn(
                name: "IsTeacherContent",
                table: "SrsDocument");
        }
    }
}
