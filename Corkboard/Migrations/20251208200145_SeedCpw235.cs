using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Corkboard.Migrations
{
    /// <inheritdoc />
    public partial class SeedCpw235 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Bio", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "Major", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePictureUrl", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "seed-user-cpw-235", 0, null, "seed-concurrency-stamp", "cpw.owner@example.com", true, "CPW Owner", false, null, "Computer Science", "CPW.OWNER@EXAMPLE.COM", "CPW.OWNER", null, null, false, null, "seed-security-stamp", false, "cpw.owner" },
                    { "seed-user-cpw-235-2", 0, null, "seed-concurrency-stamp", "cpw.student@example.com", true, "CPW Student One", false, null, "Computer Science", "CPW.STUDENT@EXAMPLE.COM", "CPW.STUDENT", null, null, false, null, "seed-security-stamp", false, "cpw.student" },
                    { "seed-user-cpw-235-3", 0, null, "seed-concurrency-stamp", "cpw.student.two@example.com", true, "CPW Student Two", false, null, "Computer Science", "CPW.STUDENT.TWO@EXAMPLE.COM", "CPW.STUDENT.TWO", null, null, false, null, "seed-security-stamp", false, "cpw.student.two" },
                    { "seed-user-cpw-235-4", 0, null, "seed-concurrency-stamp", "cpw.student.three@example.com", true, "CPW Student Three", false, null, "Software Engineering", "CPW.STUDENT.THREE@EXAMPLE.COM", "CPW.STUDENT.THREE", null, null, false, null, "seed-security-stamp", false, "cpw.student.three" },
                    { "seed-user-cpw-235-5", 0, null, "seed-concurrency-stamp", "cpw.student.four@example.com", true, "CPW Student Four", false, null, "Information Systems", "CPW.STUDENT.FOUR@EXAMPLE.COM", "CPW.STUDENT.FOUR", null, null, false, null, "seed-security-stamp", false, "cpw.student.four" },
                    { "seed-user-cpw-235-6", 0, null, "seed-concurrency-stamp", "cpw.student.five@example.com", true, "CPW Student Five", false, null, "Computer Engineering", "CPW.STUDENT.FIVE@EXAMPLE.COM", "CPW.STUDENT.FIVE", null, null, false, null, "seed-security-stamp", false, "cpw.student.five" }
                });

            migrationBuilder.InsertData(
                table: "Servers",
                columns: new[] { "Id", "CreatedAt", "Description", "IconUrl", "LastMessageTimeStamp", "Name", "OwnerId", "PrivacyLevel" },
                values: new object[] { 1, new DateTime(2024, 11, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Class discussion space for ASP.NET and computer science topics.", null, null, "CPW 235", "seed-user-cpw-235", 0 });

            migrationBuilder.InsertData(
                table: "Channels",
                columns: new[] { "Id", "CreatedAt", "Name", "ServerId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 11, 1, 12, 10, 0, 0, DateTimeKind.Utc), "general", 1 },
                    { 2, new DateTime(2024, 11, 1, 12, 11, 0, 0, DateTimeKind.Utc), "asp-net-core", 1 },
                    { 3, new DateTime(2024, 11, 1, 12, 12, 0, 0, DateTimeKind.Utc), "csharp-basics", 1 },
                    { 4, new DateTime(2024, 11, 1, 12, 13, 0, 0, DateTimeKind.Utc), "data-structures", 1 }
                });

            migrationBuilder.InsertData(
                table: "ServerInvites",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "ExpiresAt", "InviteCode", "InvitedUserId", "IsUsed", "OneTimeUse", "ServerId", "TimesUsed" },
                values: new object[] { 1, new DateTime(2024, 11, 1, 12, 30, 0, 0, DateTimeKind.Utc), "seed-user-cpw-235", null, "CPW235-TEST", null, false, false, 1, 0 });

            migrationBuilder.InsertData(
                table: "ServerMembers",
                columns: new[] { "Id", "InviteId", "JoinedAt", "Role", "ServerId", "UserId" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2024, 11, 1, 12, 5, 0, 0, DateTimeKind.Utc), 2, 1, "seed-user-cpw-235" },
                    { 2, null, new DateTime(2024, 11, 2, 9, 0, 0, 0, DateTimeKind.Utc), 0, 1, "seed-user-cpw-235-2" },
                    { 3, null, new DateTime(2024, 11, 2, 9, 5, 0, 0, DateTimeKind.Utc), 0, 1, "seed-user-cpw-235-3" },
                    { 4, null, new DateTime(2024, 11, 2, 9, 10, 0, 0, DateTimeKind.Utc), 0, 1, "seed-user-cpw-235-4" },
                    { 5, null, new DateTime(2024, 11, 2, 9, 15, 0, 0, DateTimeKind.Utc), 0, 1, "seed-user-cpw-235-5" },
                    { 6, null, new DateTime(2024, 11, 2, 9, 20, 0, 0, DateTimeKind.Utc), 0, 1, "seed-user-cpw-235-6" }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChannelId", "CreatedAt", "MessageContent", "SenderId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 11, 3, 8, 0, 0, 0, DateTimeKind.Utc), "Welcome to CPW 235! Share what you want to learn this quarter.", "seed-user-cpw-235" },
                    { 2, 1, new DateTime(2024, 11, 3, 8, 4, 0, 0, DateTimeKind.Utc), "Drop your favorite dev tool or shortcut.", "seed-user-cpw-235-2" },
                    { 3, 1, new DateTime(2024, 11, 3, 8, 8, 0, 0, DateTimeKind.Utc), "Reminder: office hours posted in #asp-net-core.", "seed-user-cpw-235-3" },
                    { 4, 1, new DateTime(2024, 11, 3, 8, 12, 0, 0, DateTimeKind.Utc), "Form study groups here—post your availability.", "seed-user-cpw-235-4" },
                    { 5, 1, new DateTime(2024, 11, 3, 8, 16, 0, 0, DateTimeKind.Utc), "What topic should we deep dive next week?", "seed-user-cpw-235-5" },
                    { 6, 1, new DateTime(2024, 11, 3, 8, 20, 0, 0, DateTimeKind.Utc), "Any blockers on the current lab?", "seed-user-cpw-235-6" },
                    { 7, 1, new DateTime(2024, 11, 3, 8, 24, 0, 0, DateTimeKind.Utc), "Share a win from today, big or small.", "seed-user-cpw-235" },
                    { 8, 1, new DateTime(2024, 11, 3, 8, 28, 0, 0, DateTimeKind.Utc), "Tips for keeping VS Code tidy?", "seed-user-cpw-235-2" },
                    { 9, 1, new DateTime(2024, 11, 3, 8, 32, 0, 0, DateTimeKind.Utc), "What podcasts or videos help you learn C#?", "seed-user-cpw-235-3" },
                    { 10, 1, new DateTime(2024, 11, 3, 8, 36, 0, 0, DateTimeKind.Utc), "If you were teaching, what's one thing you'd change?", "seed-user-cpw-235-4" },
                    { 11, 1, new DateTime(2024, 11, 3, 8, 40, 0, 0, DateTimeKind.Utc), "Which keyboard shortcut saves you the most time?", "seed-user-cpw-235-5" },
                    { 12, 1, new DateTime(2024, 11, 3, 8, 44, 0, 0, DateTimeKind.Utc), "Post a meme that sums up your debugging session.", "seed-user-cpw-235-6" },
                    { 13, 1, new DateTime(2024, 11, 3, 8, 48, 0, 0, DateTimeKind.Utc), "Who wants to pair program tonight?", "seed-user-cpw-235" },
                    { 14, 1, new DateTime(2024, 11, 3, 8, 52, 0, 0, DateTimeKind.Utc), "Share your go-to snack while coding.", "seed-user-cpw-235-2" },
                    { 15, 1, new DateTime(2024, 11, 3, 8, 56, 0, 0, DateTimeKind.Utc), "What is your terminal theme?", "seed-user-cpw-235-3" },
                    { 16, 1, new DateTime(2024, 11, 3, 9, 0, 0, 0, DateTimeKind.Utc), "Drop a screenshot of your current setup.", "seed-user-cpw-235-4" },
                    { 17, 1, new DateTime(2024, 11, 3, 9, 4, 0, 0, DateTimeKind.Utc), "Best advice you've received about learning to code?", "seed-user-cpw-235-5" },
                    { 18, 1, new DateTime(2024, 11, 3, 9, 8, 0, 0, DateTimeKind.Utc), "One concept you want reviewed this week?", "seed-user-cpw-235-6" },
                    { 19, 1, new DateTime(2024, 11, 3, 9, 12, 0, 0, DateTimeKind.Utc), "Favorite NuGet package you discovered recently?", "seed-user-cpw-235" },
                    { 20, 1, new DateTime(2024, 11, 3, 9, 16, 0, 0, DateTimeKind.Utc), "How do you keep track of tasks and todos?", "seed-user-cpw-235-2" },
                    { 21, 1, new DateTime(2024, 11, 3, 9, 20, 0, 0, DateTimeKind.Utc), "What helps you focus during deep work?", "seed-user-cpw-235-3" },
                    { 22, 1, new DateTime(2024, 11, 3, 9, 24, 0, 0, DateTimeKind.Utc), "Share a Git command you rely on daily.", "seed-user-cpw-235-4" },
                    { 23, 1, new DateTime(2024, 11, 3, 9, 28, 0, 0, DateTimeKind.Utc), "What's confusing about HTTP vs HTTPS?", "seed-user-cpw-235-5" },
                    { 24, 1, new DateTime(2024, 11, 3, 9, 32, 0, 0, DateTimeKind.Utc), "Any tips for balancing coursework and rest?", "seed-user-cpw-235-6" },
                    { 25, 1, new DateTime(2024, 11, 3, 9, 36, 0, 0, DateTimeKind.Utc), "Share a bug you solved today.", "seed-user-cpw-235" },
                    { 26, 1, new DateTime(2024, 11, 3, 9, 40, 0, 0, DateTimeKind.Utc), "What music helps you concentrate?", "seed-user-cpw-235-2" },
                    { 27, 1, new DateTime(2024, 11, 3, 9, 44, 0, 0, DateTimeKind.Utc), "How do you name variables for clarity?", "seed-user-cpw-235-3" },
                    { 28, 1, new DateTime(2024, 11, 3, 9, 48, 0, 0, DateTimeKind.Utc), "What is your favorite C# feature?", "seed-user-cpw-235-4" },
                    { 29, 1, new DateTime(2024, 11, 3, 9, 52, 0, 0, DateTimeKind.Utc), "How do you test your code locally?", "seed-user-cpw-235-5" },
                    { 30, 1, new DateTime(2024, 11, 3, 9, 56, 0, 0, DateTimeKind.Utc), "Share a small habit that improved your code quality.", "seed-user-cpw-235-6" },
                    { 31, 1, new DateTime(2024, 11, 3, 10, 0, 0, 0, DateTimeKind.Utc), "Any questions about the upcoming assignment?", "seed-user-cpw-235" },
                    { 32, 1, new DateTime(2024, 11, 3, 10, 4, 0, 0, DateTimeKind.Utc), "What do you want to build after this class?", "seed-user-cpw-235-2" },
                    { 33, 1, new DateTime(2024, 11, 3, 10, 8, 0, 0, DateTimeKind.Utc), "Share a helpful doc link you keep bookmarked.", "seed-user-cpw-235-3" },
                    { 34, 1, new DateTime(2024, 11, 3, 10, 12, 0, 0, DateTimeKind.Utc), "How do you stay organized across projects?", "seed-user-cpw-235-4" },
                    { 35, 1, new DateTime(2024, 11, 3, 10, 16, 0, 0, DateTimeKind.Utc), "What is one thing you wish you knew earlier about Git?", "seed-user-cpw-235-5" },
                    { 36, 1, new DateTime(2024, 11, 3, 10, 20, 0, 0, DateTimeKind.Utc), "Favorite VS Code extension right now?", "seed-user-cpw-235-6" },
                    { 37, 1, new DateTime(2024, 11, 3, 10, 24, 0, 0, DateTimeKind.Utc), "How do you approach refactoring safely?", "seed-user-cpw-235" },
                    { 38, 1, new DateTime(2024, 11, 3, 10, 28, 0, 0, DateTimeKind.Utc), "Share a CSS trick you like.", "seed-user-cpw-235-2" },
                    { 39, 1, new DateTime(2024, 11, 3, 10, 32, 0, 0, DateTimeKind.Utc), "What error message have you seen too often?", "seed-user-cpw-235-3" },
                    { 40, 1, new DateTime(2024, 11, 3, 10, 36, 0, 0, DateTimeKind.Utc), "How do you handle merge conflicts?", "seed-user-cpw-235-4" },
                    { 41, 1, new DateTime(2024, 11, 3, 10, 40, 0, 0, DateTimeKind.Utc), "One networking concept you want clarified?", "seed-user-cpw-235-5" },
                    { 42, 1, new DateTime(2024, 11, 3, 10, 44, 0, 0, DateTimeKind.Utc), "Share a shortcut for navigating files quickly.", "seed-user-cpw-235-6" },
                    { 43, 1, new DateTime(2024, 11, 3, 10, 48, 0, 0, DateTimeKind.Utc), "What part of SOLID is still fuzzy?", "seed-user-cpw-235" },
                    { 44, 1, new DateTime(2024, 11, 3, 10, 52, 0, 0, DateTimeKind.Utc), "Any strategies for naming branches?", "seed-user-cpw-235-2" },
                    { 45, 1, new DateTime(2024, 11, 3, 10, 56, 0, 0, DateTimeKind.Utc), "How do you document your APIs?", "seed-user-cpw-235-3" },
                    { 46, 1, new DateTime(2024, 11, 3, 11, 0, 0, 0, DateTimeKind.Utc), "Share a tip for writing better commit messages.", "seed-user-cpw-235-4" },
                    { 47, 1, new DateTime(2024, 11, 3, 11, 4, 0, 0, DateTimeKind.Utc), "What database topic should we revisit?", "seed-user-cpw-235-5" },
                    { 48, 1, new DateTime(2024, 11, 3, 11, 8, 0, 0, DateTimeKind.Utc), "How do you stay motivated on long assignments?", "seed-user-cpw-235-6" },
                    { 49, 1, new DateTime(2024, 11, 3, 11, 12, 0, 0, DateTimeKind.Utc), "Post a question you think others might have too.", "seed-user-cpw-235" },
                    { 50, 1, new DateTime(2024, 11, 3, 11, 16, 0, 0, DateTimeKind.Utc), "Welcome to CPW 235! Share what you want to learn this quarter.", "seed-user-cpw-235-2" },
                    { 51, 2, new DateTime(2024, 11, 4, 9, 0, 0, 0, DateTimeKind.Utc), "Routing and attribute routes feel clearer now.", "seed-user-cpw-235-2" },
                    { 52, 2, new DateTime(2024, 11, 4, 9, 5, 0, 0, DateTimeKind.Utc), "Remember to register SignalR in Program.cs.", "seed-user-cpw-235-3" },
                    { 53, 2, new DateTime(2024, 11, 4, 9, 10, 0, 0, DateTimeKind.Utc), "Add logging to controller actions while debugging.", "seed-user-cpw-235-4" },
                    { 54, 2, new DateTime(2024, 11, 4, 9, 15, 0, 0, DateTimeKind.Utc), "Identity scaffolding saved a ton of time.", "seed-user-cpw-235-5" },
                    { 55, 2, new DateTime(2024, 11, 4, 9, 20, 0, 0, DateTimeKind.Utc), "Static files need cache busting—enable versioning.", "seed-user-cpw-235-6" },
                    { 56, 2, new DateTime(2024, 11, 4, 9, 25, 0, 0, DateTimeKind.Utc), "Partial views keep the layout clean.", "seed-user-cpw-235" },
                    { 57, 2, new DateTime(2024, 11, 4, 9, 30, 0, 0, DateTimeKind.Utc), "Model validation attributes caught my bug.", "seed-user-cpw-235-2" },
                    { 58, 2, new DateTime(2024, 11, 4, 9, 35, 0, 0, DateTimeKind.Utc), "DI setup: keep services in Program.cs tidy.", "seed-user-cpw-235-3" },
                    { 59, 2, new DateTime(2024, 11, 4, 9, 40, 0, 0, DateTimeKind.Utc), "Swagger UI helps test quickly.", "seed-user-cpw-235-4" },
                    { 60, 2, new DateTime(2024, 11, 4, 9, 45, 0, 0, DateTimeKind.Utc), "Remember to check appsettings.Development.json.", "seed-user-cpw-235-5" },
                    { 61, 3, new DateTime(2024, 11, 5, 10, 0, 0, 0, DateTimeKind.Utc), "Interfaces finally clicked today.", "seed-user-cpw-235-3" },
                    { 62, 3, new DateTime(2024, 11, 5, 10, 6, 0, 0, DateTimeKind.Utc), "LINQ query syntax vs method syntax—team method.", "seed-user-cpw-235-4" },
                    { 63, 3, new DateTime(2024, 11, 5, 10, 12, 0, 0, DateTimeKind.Utc), "Records are great for DTOs.", "seed-user-cpw-235-5" },
                    { 64, 3, new DateTime(2024, 11, 5, 10, 18, 0, 0, DateTimeKind.Utc), "Pattern matching made my switch smaller.", "seed-user-cpw-235-6" },
                    { 65, 3, new DateTime(2024, 11, 5, 10, 24, 0, 0, DateTimeKind.Utc), "Nullable reference types caught a null bug.", "seed-user-cpw-235" },
                    { 66, 3, new DateTime(2024, 11, 5, 10, 30, 0, 0, DateTimeKind.Utc), "Async/await still trips me up.", "seed-user-cpw-235-2" },
                    { 67, 3, new DateTime(2024, 11, 5, 10, 36, 0, 0, DateTimeKind.Utc), "Extension methods keep helpers organized.", "seed-user-cpw-235-3" },
                    { 68, 3, new DateTime(2024, 11, 5, 10, 42, 0, 0, DateTimeKind.Utc), "String interpolation over concatenation any day.", "seed-user-cpw-235-4" },
                    { 69, 3, new DateTime(2024, 11, 5, 10, 48, 0, 0, DateTimeKind.Utc), "Use var judiciously; clarity first.", "seed-user-cpw-235-5" },
                    { 70, 3, new DateTime(2024, 11, 5, 10, 54, 0, 0, DateTimeKind.Utc), "xml comments help IntelliSense a lot.", "seed-user-cpw-235-6" },
                    { 71, 4, new DateTime(2024, 11, 6, 11, 0, 0, 0, DateTimeKind.Utc), "1: Queues vs stacks: when would you pick each?", "seed-user-cpw-235-4" },
                    { 72, 4, new DateTime(2024, 11, 6, 11, 7, 0, 0, DateTimeKind.Utc), "2: Binary search trees feel elegant.", "seed-user-cpw-235-5" },
                    { 73, 4, new DateTime(2024, 11, 6, 11, 14, 0, 0, DateTimeKind.Utc), "3: Linked lists are great for constant-time inserts.", "seed-user-cpw-235-6" },
                    { 74, 4, new DateTime(2024, 11, 6, 11, 21, 0, 0, DateTimeKind.Utc), "4: Hash sets saved me from duplicates.", "seed-user-cpw-235" },
                    { 75, 4, new DateTime(2024, 11, 6, 11, 28, 0, 0, DateTimeKind.Utc), "5: Graph traversal with BFS felt natural.", "seed-user-cpw-235-2" },
                    { 76, 4, new DateTime(2024, 11, 6, 11, 35, 0, 0, DateTimeKind.Utc), "6: DFS recursion depth is a gotcha.", "seed-user-cpw-235-3" },
                    { 77, 4, new DateTime(2024, 11, 6, 11, 42, 0, 0, DateTimeKind.Utc), "7: Big-O cheat sheet taped to my monitor.", "seed-user-cpw-235-4" },
                    { 78, 4, new DateTime(2024, 11, 6, 11, 49, 0, 0, DateTimeKind.Utc), "8: Priority queues make scheduling easier.", "seed-user-cpw-235-5" },
                    { 79, 4, new DateTime(2024, 11, 6, 11, 56, 0, 0, DateTimeKind.Utc), "9: Topological sort finally clicked!", "seed-user-cpw-235-6" },
                    { 80, 4, new DateTime(2024, 11, 6, 12, 3, 0, 0, DateTimeKind.Utc), "10: Two-pointer patterns help in arrays.", "seed-user-cpw-235" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "ServerInvites",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServerMembers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServerMembers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ServerMembers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ServerMembers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ServerMembers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ServerMembers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "seed-user-cpw-235-2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "seed-user-cpw-235-3");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "seed-user-cpw-235-4");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "seed-user-cpw-235-5");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "seed-user-cpw-235-6");

            migrationBuilder.DeleteData(
                table: "Channels",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Channels",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Channels",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Channels",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Servers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "seed-user-cpw-235");
        }
    }
}
