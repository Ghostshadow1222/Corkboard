using Corkboard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Corkboard.Data;

public class ApplicationDbContext : IdentityDbContext<UserAccount>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}

	public DbSet<Server> Servers { get; set; }
	public DbSet<ServerMember> ServerMembers { get; set; }
	public DbSet<ServerInvite> ServerInvites { get; set; }
	public DbSet<Channel> Channels { get; set; }
	public DbSet<Message> Messages { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		// Seed a demo server with channels, messages, invites, and membership.
		const string ownerId = "seed-user-cpw-235";
		const string studentId = "seed-user-cpw-235-2";
		const string studentId3 = "seed-user-cpw-235-3";
		const string studentId4 = "seed-user-cpw-235-4";
		const string studentId5 = "seed-user-cpw-235-5";
		const string studentId6 = "seed-user-cpw-235-6";
		const int serverId = 1;
		const int generalChannelId = 1;
		const int aspNetChannelId = 2;
		const int csharpChannelId = 3;
		const int dataStructuresChannelId = 4;

		builder.Entity<UserAccount>().HasData(
			new UserAccount
			{
				Id = ownerId,
				UserName = "cpw.owner",
				NormalizedUserName = "CPW.OWNER",
				Email = "cpw.owner@example.com",
				NormalizedEmail = "CPW.OWNER@EXAMPLE.COM",
				EmailConfirmed = true,
				SecurityStamp = "seed-security-stamp",
				ConcurrencyStamp = "seed-concurrency-stamp",
				FullName = "CPW Owner",
				Major = "Computer Science"
			},
			new UserAccount
			{
				Id = studentId,
				UserName = "cpw.student",
				NormalizedUserName = "CPW.STUDENT",
				Email = "cpw.student@example.com",
				NormalizedEmail = "CPW.STUDENT@EXAMPLE.COM",
				EmailConfirmed = true,
				SecurityStamp = "seed-security-stamp",
				ConcurrencyStamp = "seed-concurrency-stamp",
				FullName = "CPW Student One",
				Major = "Computer Science"
			},
			new UserAccount
			{
				Id = studentId3,
				UserName = "cpw.student.two",
				NormalizedUserName = "CPW.STUDENT.TWO",
				Email = "cpw.student.two@example.com",
				NormalizedEmail = "CPW.STUDENT.TWO@EXAMPLE.COM",
				EmailConfirmed = true,
				SecurityStamp = "seed-security-stamp",
				ConcurrencyStamp = "seed-concurrency-stamp",
				FullName = "CPW Student Two",
				Major = "Computer Science"
			},
			new UserAccount
			{
				Id = studentId4,
				UserName = "cpw.student.three",
				NormalizedUserName = "CPW.STUDENT.THREE",
				Email = "cpw.student.three@example.com",
				NormalizedEmail = "CPW.STUDENT.THREE@EXAMPLE.COM",
				EmailConfirmed = true,
				SecurityStamp = "seed-security-stamp",
				ConcurrencyStamp = "seed-concurrency-stamp",
				FullName = "CPW Student Three",
				Major = "Software Engineering"
			},
			new UserAccount
			{
				Id = studentId5,
				UserName = "cpw.student.four",
				NormalizedUserName = "CPW.STUDENT.FOUR",
				Email = "cpw.student.four@example.com",
				NormalizedEmail = "CPW.STUDENT.FOUR@EXAMPLE.COM",
				EmailConfirmed = true,
				SecurityStamp = "seed-security-stamp",
				ConcurrencyStamp = "seed-concurrency-stamp",
				FullName = "CPW Student Four",
				Major = "Information Systems"
			},
			new UserAccount
			{
				Id = studentId6,
				UserName = "cpw.student.five",
				NormalizedUserName = "CPW.STUDENT.FIVE",
				Email = "cpw.student.five@example.com",
				NormalizedEmail = "CPW.STUDENT.FIVE@EXAMPLE.COM",
				EmailConfirmed = true,
				SecurityStamp = "seed-security-stamp",
				ConcurrencyStamp = "seed-concurrency-stamp",
				FullName = "CPW Student Five",
				Major = "Computer Engineering"
			}
		);

		builder.Entity<Server>().HasData(new Server
		{
			Id = serverId,
			Name = "CPW 235",
			Description = "Class discussion space for ASP.NET and computer science topics.",
			IconUrl = null,
			OwnerId = ownerId,
			PrivacyLevel = PrivacyLevel.Public,
			CreatedAt = new DateTime(2024, 11, 1, 12, 0, 0, DateTimeKind.Utc)
		});

		builder.Entity<ServerMember>().HasData(
			new ServerMember
			{
				Id = 1,
				ServerId = serverId,
				UserId = ownerId,
				Role = RoleType.Owner,
				JoinedAt = new DateTime(2024, 11, 1, 12, 5, 0, DateTimeKind.Utc)
			},
			new ServerMember
			{
				Id = 2,
				ServerId = serverId,
				UserId = studentId,
				Role = RoleType.Member,
				JoinedAt = new DateTime(2024, 11, 2, 9, 0, 0, DateTimeKind.Utc)
			},
			new ServerMember
			{
				Id = 3,
				ServerId = serverId,
				UserId = studentId3,
				Role = RoleType.Member,
				JoinedAt = new DateTime(2024, 11, 2, 9, 5, 0, DateTimeKind.Utc)
			},
			new ServerMember
			{
				Id = 4,
				ServerId = serverId,
				UserId = studentId4,
				Role = RoleType.Member,
				JoinedAt = new DateTime(2024, 11, 2, 9, 10, 0, DateTimeKind.Utc)
			},
			new ServerMember
			{
				Id = 5,
				ServerId = serverId,
				UserId = studentId5,
				Role = RoleType.Member,
				JoinedAt = new DateTime(2024, 11, 2, 9, 15, 0, DateTimeKind.Utc)
			},
			new ServerMember
			{
				Id = 6,
				ServerId = serverId,
				UserId = studentId6,
				Role = RoleType.Member,
				JoinedAt = new DateTime(2024, 11, 2, 9, 20, 0, DateTimeKind.Utc)
			}
		);

		builder.Entity<Channel>().HasData(
			new Channel
			{
				Id = generalChannelId,
				ServerId = serverId,
				Name = "general",
				CreatedAt = new DateTime(2024, 11, 1, 12, 10, 0, DateTimeKind.Utc)
			},
			new Channel
			{
				Id = aspNetChannelId,
				ServerId = serverId,
				Name = "asp-net-core",
				CreatedAt = new DateTime(2024, 11, 1, 12, 11, 0, DateTimeKind.Utc)
			},
			new Channel
			{
				Id = csharpChannelId,
				ServerId = serverId,
				Name = "csharp-basics",
				CreatedAt = new DateTime(2024, 11, 1, 12, 12, 0, DateTimeKind.Utc)
			},
			new Channel
			{
				Id = dataStructuresChannelId,
				ServerId = serverId,
				Name = "data-structures",
				CreatedAt = new DateTime(2024, 11, 1, 12, 13, 0, DateTimeKind.Utc)
			}
		);

		List<Message> messages = new List<Message>();
		string[] authors = { ownerId, studentId, studentId3, studentId4, studentId5, studentId6 };
		int messageId = 1;
		DateTime generalStart = new DateTime(2024, 11, 3, 8, 0, 0, DateTimeKind.Utc);
		string[] generalPrompts =
		{
			"Welcome to CPW 235! Share what you want to learn this quarter.",
			"Drop your favorite dev tool or shortcut.",
			"Reminder: office hours posted in #asp-net-core.",
			"Form study groups here—post your availability.",
			"What topic should we deep dive next week?",
			"Any blockers on the current lab?",
			"Share a win from today, big or small.",
			"Tips for keeping VS Code tidy?",
			"What podcasts or videos help you learn C#?",
			"If you were teaching, what's one thing you'd change?",
			"Which keyboard shortcut saves you the most time?",
			"Post a meme that sums up your debugging session.",
			"Who wants to pair program tonight?",
			"Share your go-to snack while coding.",
			"What is your terminal theme?",
			"Drop a screenshot of your current setup.",
			"Best advice you've received about learning to code?",
			"One concept you want reviewed this week?",
			"Favorite NuGet package you discovered recently?",
			"How do you keep track of tasks and todos?",
			"What helps you focus during deep work?",
			"Share a Git command you rely on daily.",
			"What's confusing about HTTP vs HTTPS?",
			"Any tips for balancing coursework and rest?",
			"Share a bug you solved today.",
			"What music helps you concentrate?",
			"How do you name variables for clarity?",
			"What is your favorite C# feature?",
			"How do you test your code locally?",
			"Share a small habit that improved your code quality.",
			"Any questions about the upcoming assignment?",
			"What do you want to build after this class?",
			"Share a helpful doc link you keep bookmarked.",
			"How do you stay organized across projects?",
			"What is one thing you wish you knew earlier about Git?",
			"Favorite VS Code extension right now?",
			"How do you approach refactoring safely?",
			"Share a CSS trick you like.",
			"What error message have you seen too often?",
			"How do you handle merge conflicts?",
			"One networking concept you want clarified?",
			"Share a shortcut for navigating files quickly.",
			"What part of SOLID is still fuzzy?",
			"Any strategies for naming branches?",
			"How do you document your APIs?",
			"Share a tip for writing better commit messages.",
			"What database topic should we revisit?",
			"How do you stay motivated on long assignments?",
			"Post a question you think others might have too."
		};

		for (int i = 0; i < 50; i++)
		{
			messages.Add(new Message
			{
				Id = messageId++,
				ChannelId = generalChannelId,
				SenderId = authors[i % authors.Length],
				MessageContent = $"{generalPrompts[i % generalPrompts.Length]}",
				CreatedAt = generalStart.AddMinutes(i * 4)
			});
		}

		DateTime aspNetStart = new DateTime(2024, 11, 4, 9, 0, 0, DateTimeKind.Utc);
		string[] aspTopics =
		{
			"Routing and attribute routes feel clearer now.",
			"Remember to register SignalR in Program.cs.",
			"Add logging to controller actions while debugging.",
			"Identity scaffolding saved a ton of time.",
			"Static files need cache busting—enable versioning.",
			"Partial views keep the layout clean.",
			"Model validation attributes caught my bug.",
			"DI setup: keep services in Program.cs tidy.",
			"Swagger UI helps test quickly.",
			"Remember to check appsettings.Development.json."
		};
		for (int i = 0; i < 10; i++)
		{
			messages.Add(new Message
			{
				Id = messageId++,
				ChannelId = aspNetChannelId,
				SenderId = authors[(i + 1) % authors.Length],
				MessageContent = $"{aspTopics[i % aspTopics.Length]}",
				CreatedAt = aspNetStart.AddMinutes(i * 5)
			});
		}

		DateTime csharpStart = new DateTime(2024, 11, 5, 10, 0, 0, DateTimeKind.Utc);
		string[] csharpTopics =
		{
			"Interfaces finally clicked today.",
			"LINQ query syntax vs method syntax—team method.",
			"Records are great for DTOs.",
			"Pattern matching made my switch smaller.",
			"Nullable reference types caught a null bug.",
			"Async/await still trips me up.",
			"Extension methods keep helpers organized.",
			"String interpolation over concatenation any day.",
			"Use var judiciously; clarity first.",
			"xml comments help IntelliSense a lot."
		};
		for (int i = 0; i < 10; i++)
		{
			messages.Add(new Message
			{
				Id = messageId++,
				ChannelId = csharpChannelId,
				SenderId = authors[(i + 2) % authors.Length],
				MessageContent = $"{csharpTopics[i % csharpTopics.Length]}",
				CreatedAt = csharpStart.AddMinutes(i * 6)
			});
		}

		DateTime dsStart = new DateTime(2024, 11, 6, 11, 0, 0, DateTimeKind.Utc);
		string[] dsTopics =
		{
			"Queues vs stacks: when would you pick each?",
			"Binary search trees feel elegant.",
			"Linked lists are great for constant-time inserts.",
			"Hash sets saved me from duplicates.",
			"Graph traversal with BFS felt natural.",
			"DFS recursion depth is a gotcha.",
			"Big-O cheat sheet taped to my monitor.",
			"Priority queues make scheduling easier.",
			"Topological sort finally clicked!",
			"Two-pointer patterns help in arrays."
		};
		for (int i = 0; i < 10; i++)
		{
			messages.Add(new Message
			{
				Id = messageId++,
				ChannelId = dataStructuresChannelId,
				SenderId = authors[(i + 3) % authors.Length],
				MessageContent = $"{i + 1}: {dsTopics[i % dsTopics.Length]}",
				CreatedAt = dsStart.AddMinutes(i * 7)
			});
		}

		builder.Entity<Message>().HasData(messages);

		builder.Entity<ServerInvite>().HasData(
			new ServerInvite
			{
				Id = 1,
				ServerId = serverId,
				CreatedById = ownerId,
				InviteCode = "CPW235-TEST",
				CreatedAt = new DateTime(2024, 11, 1, 12, 30, 0, DateTimeKind.Utc),
				ExpiresAt = null,
				OneTimeUse = false,
				IsUsed = false,
				TimesUsed = 0
			}
		);
	}
}
