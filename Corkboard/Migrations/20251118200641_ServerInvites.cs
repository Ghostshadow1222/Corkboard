using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corkboard.Migrations
{
    /// <inheritdoc />
    public partial class ServerInvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrivacyLevel",
                table: "Servers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "ServerMembers",
                type: "INTEGER",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "ServerInvites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatorId = table.Column<string>(type: "TEXT", nullable: false),
                    InvitedUserId = table.Column<string>(type: "TEXT", nullable: true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OneTimeUse = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsUsed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TimesUsed = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerInvites_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServerInvites_AspNetUsers_InvitedUserId",
                        column: x => x.InvitedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServerInvites_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServerInvites_CreatorId",
                table: "ServerInvites",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerInvites_InvitedUserId",
                table: "ServerInvites",
                column: "InvitedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerInvites_ServerId",
                table: "ServerInvites",
                column: "ServerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerInvites");

            migrationBuilder.DropColumn(
                name: "PrivacyLevel",
                table: "Servers");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "ServerMembers",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldMaxLength: 50);
        }
    }
}
