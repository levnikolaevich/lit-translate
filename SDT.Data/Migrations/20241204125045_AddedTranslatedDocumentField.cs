using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDT.LData.Migrations
{
    /// <inheritdoc />
    public partial class AddedTranslatedDocumentField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TranslatedDocumentId",
                table: "TranslationTasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TranslationTasks_TranslatedDocumentId",
                table: "TranslationTasks",
                column: "TranslatedDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationTasks_Documents_TranslatedDocumentId",
                table: "TranslationTasks",
                column: "TranslatedDocumentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TranslationTasks_Documents_TranslatedDocumentId",
                table: "TranslationTasks");

            migrationBuilder.DropIndex(
                name: "IX_TranslationTasks_TranslatedDocumentId",
                table: "TranslationTasks");

            migrationBuilder.DropColumn(
                name: "TranslatedDocumentId",
                table: "TranslationTasks");
        }
    }
}
