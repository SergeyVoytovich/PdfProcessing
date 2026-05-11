using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PdfProcessing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPageNumberToDocumentContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageNumber",
                table: "document_contents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageNumber",
                table: "document_contents");
        }
    }
}
