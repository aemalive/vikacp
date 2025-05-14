using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cp.Migrations
{
    /// <inheritdoc />
    public partial class editCartItemAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                table: "CartItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "CartItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
