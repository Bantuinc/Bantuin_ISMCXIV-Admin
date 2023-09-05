using Bantuin_ISMCXIV_Admin.Data;
using Bantuin_ISMCXIV_Admin.Data.Enum;
using Bantuin_ISMCXIV_Admin.Models;
using Microsoft.AspNetCore.Identity;

namespace Bantuin_ISMCXIV_Admin
{
	public class Seed
	{
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly DataContext _context;

		public Seed(UserManager<User> userManager, 
			RoleManager<IdentityRole> roleManager,
			DataContext context)
        {
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
		}

		/* * * * * *  Login Data * * * * * *
		 * Admin						   *
		 * Email	: admin@admin.com	   *
		 * Password	: antihoaks123		   *
		 *								   *
		 * User							   *
		 * Email	: usertes1@example.com *
		 * Password	: walangkecek123	   *
		 * Email	: usertes2@example.com *
		 * Password	: walangkecek123	   *
		 * * * * * * * * * * * * * * * * * */
		public async Task SeedDataContextAsync()
		{
			if (!await _roleManager.RoleExistsAsync("Admin"))
			{
				await _roleManager.CreateAsync(new IdentityRole("Admin"));
				await _roleManager.CreateAsync(new IdentityRole("User"));

				var admin = new User
				{
					FullName = "Admin",
					UserName = "admin",
					Email = "admin@admin.com"
				};
				var addAdmin = await _userManager.CreateAsync(admin, "antihoaks123");
				if (addAdmin.Succeeded)
				{
					await _userManager.AddToRoleAsync(admin, "Admin");
					Console.WriteLine($"User {admin.UserName} created successfully.");
				}
				else
				{
					Console.WriteLine($"User {admin.UserName} failed to create.");
				}

				var userNew = new List<User>()
				{
					new User
					{
						FullName = "yahaha",
						UserName = "tes123",
						Email = "usertes1@example.com"
					},
					new User
					{
						FullName = "hayukkk",
						UserName = "tes234",
						Email = "usertes2@example.com"
					},
				};
				foreach (var users in userNew)
				{
					try
					{
						await _userManager.CreateAsync(users, "walangkecek123");
						await _userManager.AddToRoleAsync(users, "User");
						Console.WriteLine($"User {users.UserName} created successfully.");
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
				var user1 = await _userManager.FindByEmailAsync("usertes1@example.com");
				var user2 = await _userManager.FindByEmailAsync("usertes2@example.com");

				var newEvent = new Event
				{
					Name = "ISMC",
					Description = "Event 1",
					Picture = "oce maaf",
					StartDate = DateTime.UtcNow,
					EndDate = DateTime.UtcNow.AddDays(30),
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow,

				};

				var newTeam = new Team
				{
					Name = "Infinity Free",
					University = "Universitas Indonesia",
					StreetAddress = "Jl. Margonda Raya No. 1",
					City = "Depok",
					State = "Jawa Barat",
					Country = "Indonesia",
					PostalCode = "16424",
					Event = newEvent,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow,
				};
				_context.Teams.Add(newTeam);
				await _context.SaveChangesAsync();
				Console.WriteLine($"Event {newEvent.Name} and Team {newTeam.Name} created successfully.");

				var team = _context.Teams.Where(x => x.Name == "Infinity Free").FirstOrDefault();

				var newTeamMember = new List<TeamMember>
				{
					new TeamMember
					{
						Name = "Agil Fuad",
						Nim = "1234567890",
						Email = "usertes1@example.com",
						Phone = "081234567890",
						Ktm = "oce",
						Team = team,
						User = user1,
						Role = (TeamMemberRole)Enum.ToObject(typeof(TeamMemberRole), 1),
						Status = Data.Enum.TeamMemberStatus.Accepted,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow,
					},
					new TeamMember
					{
						Name = "Bintang Solo",
						Nim = "1234324389",
						Email = "usertes2@example.com",
						Phone = "081234567890",
						Ktm = "oceeee",
						Team = team,
						User = user2,
						Role = Data.Enum.TeamMemberRole.Member,
						Status = Data.Enum.TeamMemberStatus.OnReview,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow,
					}
				};
				_context.TeamMembers.AddRange(newTeamMember);
				await _context.SaveChangesAsync();
				Console.WriteLine($"TeamMember {newTeamMember[0].Name} and {newTeamMember[1].Name} created successfully.");
				Console.WriteLine("seeding done");

			}
			else
			{
				Console.WriteLine("already seeded");
			}
		}
	}
}
