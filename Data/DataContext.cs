using Bantuin_ISMCXIV_Admin.Data.Enum;
using Bantuin_ISMCXIV_Admin.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bantuin_ISMCXIV_Admin.Data
{
	public class DataContext : IdentityDbContext<User>
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options) { }
		public DbSet<Team> Teams { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<TeamMember> TeamMembers { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
