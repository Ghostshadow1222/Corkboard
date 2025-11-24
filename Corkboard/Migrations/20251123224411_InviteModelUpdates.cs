using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corkboard.Migrations
{
    /// <inheritdoc />
    public partial class InviteModelUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServerInvites_AspNetUsers_CreatorId",
                table: "ServerInvites");

            migrationBuilder.DropIndex(
                name: "IX_ServerInvites_CreatorId",
                table: "ServerInvites");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "ServerInvites",
                newName: "InviteCode");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "ServerInvites",
                newName: "CreatedById");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresAt",
                table: "ServerInvites",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_ServerInvites_CreatedById",
                table: "ServerInvites",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerInvites_AspNetUsers_CreatedById",
                table: "ServerInvites",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServerInvites_AspNetUsers_CreatedById",
                table: "ServerInvites");

            migrationBuilder.DropIndex(
                name: "IX_ServerInvites_CreatedById",
                table: "ServerInvites");

            migrationBuilder.RenameColumn(
                name: "InviteCode",
                table: "ServerInvites",
                newName: "CreatorId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "ServerInvites",
                newName: "Code");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresAt",
                table: "ServerInvites",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServerInvites_CreatorId",
                table: "ServerInvites",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerInvites_AspNetUsers_CreatorId",
                table: "ServerInvites",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
