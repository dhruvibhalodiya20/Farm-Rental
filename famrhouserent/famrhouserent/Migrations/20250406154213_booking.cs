using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace famrhouserent.Migrations
{
    /// <inheritdoc />
    public partial class booking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FarmHouses",
                table: "FarmHouses");

            migrationBuilder.RenameTable(
                name: "FarmHouses",
                newName: "FarmHouse");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FarmHouse",
                table: "FarmHouse",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "tblBooking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FarmHouseId = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    BookingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBooking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblBooking_FarmHouse_FarmHouseId",
                        column: x => x.FarmHouseId,
                        principalTable: "FarmHouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblBooking_UserAccounts_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblBooking_FarmHouseId",
                table: "tblBooking",
                column: "FarmHouseId");

            migrationBuilder.CreateIndex(
                name: "IX_tblBooking_UserId",
                table: "tblBooking",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblBooking");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FarmHouse",
                table: "FarmHouse");

            migrationBuilder.RenameTable(
                name: "FarmHouse",
                newName: "FarmHouses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FarmHouses",
                table: "FarmHouses",
                column: "Id");
        }
    }
}
