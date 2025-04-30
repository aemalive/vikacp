using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cp.Migrations
{
    /// <inheritdoc />
    public partial class AddFlower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Flowers",
                columns: new[] { "Id", "Name", "Price", "Quantity" },
                values: new object[] { 1, "flower1", 123m, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
