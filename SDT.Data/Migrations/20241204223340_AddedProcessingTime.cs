using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDT.LData.Migrations
{
    /// <inheritdoc />
    public partial class AddedProcessingTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProcessingTime",
                table: "TranslationTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingTime",
                table: "ParagraphTranslations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessingTime",
                table: "TranslationTasks");

            migrationBuilder.DropColumn(
                name: "ProcessingTime",
                table: "ParagraphTranslations");
        }
    }
}
