using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFlower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Flowers");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Flowers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Flowers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "ImageURL" },
                values: new object[] { "Пример цветка", "https://example.com/image.jpg" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Flowers");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Flowers");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Flowers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Quantity",
                value: 2);
        }
    }
}
