using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace famrhouserent.Migrations
{
    /// <inheritdoc />
    public partial class owner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "FarmHouse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "tblOwner",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblOwner", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FarmHouse_OwnerId",
                table: "FarmHouse",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmHouse_tblOwner_OwnerId",
                table: "FarmHouse",
                column: "OwnerId",
                principalTable: "tblOwner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmHouse_tblOwner_OwnerId",
                table: "FarmHouse");

            migrationBuilder.DropTable(
                name: "tblOwner");

            migrationBuilder.DropIndex(
                name: "IX_FarmHouse_OwnerId",
                table: "FarmHouse");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "FarmHouse");
        }
    }
}
