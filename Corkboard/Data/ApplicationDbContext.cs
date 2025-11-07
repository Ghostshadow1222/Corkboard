using Corkboard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Corkboard.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserAccount>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

		public DbSet<Server> Servers { get; set; }
		public DbSet<ServerMember> ServerMembers { get; set; }
		public DbSet<Channel> Channels { get; set; }
		public DbSet<Message> Messages { get; set; }
	}
}
