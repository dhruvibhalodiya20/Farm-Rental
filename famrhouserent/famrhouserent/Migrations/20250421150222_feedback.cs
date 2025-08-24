using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace famrhouserent.Migrations
{
    /// <inheritdoc />
    public partial class feedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_UserAccounts_UserId",
                table: "Contacts");

            migrationBuilder.CreateTable(
                name: "tblFeedback",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFeedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFeedback_tblBooking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "tblBooking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblFeedback_BookingId",
                table: "tblFeedback",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_UserAccounts_UserId",
                table: "Contacts",
                column: "UserId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_UserAccounts_UserId",
                table: "Contacts");

            migrationBuilder.DropTable(
                name: "tblFeedback");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_UserAccounts_UserId",
                table: "Contacts",
                column: "UserId",
                principalTable: "UserAccounts",
                principalColumn: "Id");
        }
    }
}
