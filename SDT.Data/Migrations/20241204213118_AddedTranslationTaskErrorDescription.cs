using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDT.LData.Migrations
{
    /// <inheritdoc />
    public partial class AddedTranslationTaskErrorDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorDescription",
                table: "TranslationTasks",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorDescription",
                table: "TranslationTasks");
        }
    }
}
