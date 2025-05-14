using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cp.Migrations
{
    /// <inheritdoc />
    public partial class editReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FlowerId",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_FlowerId",
                table: "Reviews",
                column: "FlowerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Flowers_FlowerId",
                table: "Reviews",
                column: "FlowerId",
                principalTable: "Flowers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Flowers_FlowerId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_FlowerId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "FlowerId",
                table: "Reviews");
        }
    }
}
