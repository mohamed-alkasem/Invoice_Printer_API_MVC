using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invoice_printer.Migrations
{
    /// <inheritdoc />
    public partial class migo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequiredCustomFields",
                table: "Templates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DynamicData",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredCustomFields",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "DynamicData",
                table: "Receipts");
        }
    }
}
