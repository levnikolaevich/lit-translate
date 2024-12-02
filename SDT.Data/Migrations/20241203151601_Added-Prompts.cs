using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDT.LData.Migrations
{
    /// <inheritdoc />
    public partial class AddedPrompts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "TranslationTasks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "TranslationTasks");
        }
    }
}
