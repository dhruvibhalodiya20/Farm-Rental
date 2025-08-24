using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace famrhouserent.Migrations
{
    /// <inheritdoc />
    public partial class replay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmHouseImages_FarmHouse_FarmHouseId1",
                table: "FarmHouseImages");

            migrationBuilder.DropIndex(
                name: "IX_FarmHouseImages_FarmHouseId1",
                table: "FarmHouseImages");

            migrationBuilder.DropColumn(
                name: "FarmHouseId1",
                table: "FarmHouseImages");

            migrationBuilder.AddColumn<string>(
                name: "AdminReply",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminReply",
                table: "Contacts");

            migrationBuilder.AddColumn<int>(
                name: "FarmHouseId1",
                table: "FarmHouseImages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FarmHouseImages_FarmHouseId1",
                table: "FarmHouseImages",
                column: "FarmHouseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmHouseImages_FarmHouse_FarmHouseId1",
                table: "FarmHouseImages",
                column: "FarmHouseId1",
                principalTable: "FarmHouse",
                principalColumn: "Id");
        }
    }
}
