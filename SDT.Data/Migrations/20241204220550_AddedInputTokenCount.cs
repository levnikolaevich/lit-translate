using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDT.LData.Migrations
{
    /// <inheritdoc />
    public partial class AddedInputTokenCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FinalTokenCount",
                table: "ParagraphTranslations",
                newName: "OutputTokenCount");

            migrationBuilder.AddColumn<int>(
                name: "InputTokenCount",
                table: "ParagraphTranslations",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InputTokenCount",
                table: "ParagraphTranslations");

            migrationBuilder.RenameColumn(
                name: "OutputTokenCount",
                table: "ParagraphTranslations",
                newName: "FinalTokenCount");
        }
    }
}
