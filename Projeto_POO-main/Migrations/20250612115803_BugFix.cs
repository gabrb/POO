using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFCE.Migrations
{
    /// <inheritdoc />
    public partial class BugFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Produtos_FlorId",
                table: "Produtos");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_FlorId",
                table: "Produtos",
                column: "FlorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Produtos_FlorId",
                table: "Produtos");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_FlorId",
                table: "Produtos",
                column: "FlorId",
                unique: true);
        }
    }
}
