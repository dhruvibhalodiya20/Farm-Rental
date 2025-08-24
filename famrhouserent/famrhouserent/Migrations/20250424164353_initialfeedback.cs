using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace famrhouserent.Migrations
{
    /// <inheritdoc />
    public partial class initialfeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblFeedback_tblBooking_BookingId",
                table: "tblFeedback");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "tblFeedback",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_tblFeedback_BookingId",
                table: "tblFeedback",
                newName: "IX_tblFeedback_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tblFeedback_UserAccounts_UserId",
                table: "tblFeedback",
                column: "UserId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblFeedback_UserAccounts_UserId",
                table: "tblFeedback");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "tblFeedback",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_tblFeedback_UserId",
                table: "tblFeedback",
                newName: "IX_tblFeedback_BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_tblFeedback_tblBooking_BookingId",
                table: "tblFeedback",
                column: "BookingId",
                principalTable: "tblBooking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
