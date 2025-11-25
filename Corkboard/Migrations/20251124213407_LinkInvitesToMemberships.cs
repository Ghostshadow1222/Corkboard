using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corkboard.Migrations
{
    /// <inheritdoc />
    public partial class LinkInvitesToMemberships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InviteId",
                table: "ServerMembers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServerMembers_InviteId",
                table: "ServerMembers",
                column: "InviteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerMembers_ServerInvites_InviteId",
                table: "ServerMembers",
                column: "InviteId",
                principalTable: "ServerInvites",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServerMembers_ServerInvites_InviteId",
                table: "ServerMembers");

            migrationBuilder.DropIndex(
                name: "IX_ServerMembers_InviteId",
                table: "ServerMembers");

            migrationBuilder.DropColumn(
                name: "InviteId",
                table: "ServerMembers");
        }
    }
}
