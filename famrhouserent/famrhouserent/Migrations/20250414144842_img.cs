using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace famrhouserent.Migrations
{
    /// <inheritdoc />
    public partial class img : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FarmHouseId1",
                table: "FarmHouseImages",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FarmHouse",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FarmHouse",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FarmHouse",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FarmHouse",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
