using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corkboard.Migrations
{
    /// <inheritdoc />
    public partial class ServerMembershipsAndInvitesUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServerMembers_ServerId",
                table: "ServerMembers");

            migrationBuilder.CreateIndex(
                name: "IX_ServerMembers_ServerId_UserId",
                table: "ServerMembers",
                columns: new[] { "ServerId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServerInvites_InviteCode",
                table: "ServerInvites",
                column: "InviteCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServerMembers_ServerId_UserId",
                table: "ServerMembers");

            migrationBuilder.DropIndex(
                name: "IX_ServerInvites_InviteCode",
                table: "ServerInvites");

            migrationBuilder.CreateIndex(
                name: "IX_ServerMembers_ServerId",
                table: "ServerMembers",
                column: "ServerId");
        }
    }
}
