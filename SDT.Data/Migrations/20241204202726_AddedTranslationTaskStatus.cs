using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDT.LData.Migrations
{
    /// <inheritdoc />
    public partial class AddedTranslationTaskStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TranslationTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TranslationTasks");
        }
    }
}
