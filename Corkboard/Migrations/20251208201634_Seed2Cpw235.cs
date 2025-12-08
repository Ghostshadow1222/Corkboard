using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corkboard.Migrations
{
    /// <inheritdoc />
    public partial class Seed2Cpw235 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 71,
                column: "MessageContent",
                value: "Queues vs stacks: when would you pick each?");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 72,
                column: "MessageContent",
                value: "Binary search trees feel elegant.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 73,
                column: "MessageContent",
                value: "Linked lists are great for constant-time inserts.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 74,
                column: "MessageContent",
                value: "Hash sets saved me from duplicates.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 75,
                column: "MessageContent",
                value: "Graph traversal with BFS felt natural.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 76,
                column: "MessageContent",
                value: "DFS recursion depth is a gotcha.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 77,
                column: "MessageContent",
                value: "Big-O cheat sheet taped to my monitor.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 78,
                column: "MessageContent",
                value: "Priority queues make scheduling easier.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 79,
                column: "MessageContent",
                value: "Topological sort finally clicked!");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 80,
                column: "MessageContent",
                value: "Two-pointer patterns help in arrays.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 71,
                column: "MessageContent",
                value: "1: Queues vs stacks: when would you pick each?");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 72,
                column: "MessageContent",
                value: "2: Binary search trees feel elegant.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 73,
                column: "MessageContent",
                value: "3: Linked lists are great for constant-time inserts.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 74,
                column: "MessageContent",
                value: "4: Hash sets saved me from duplicates.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 75,
                column: "MessageContent",
                value: "5: Graph traversal with BFS felt natural.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 76,
                column: "MessageContent",
                value: "6: DFS recursion depth is a gotcha.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 77,
                column: "MessageContent",
                value: "7: Big-O cheat sheet taped to my monitor.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 78,
                column: "MessageContent",
                value: "8: Priority queues make scheduling easier.");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 79,
                column: "MessageContent",
                value: "9: Topological sort finally clicked!");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 80,
                column: "MessageContent",
                value: "10: Two-pointer patterns help in arrays.");
        }
    }
}
