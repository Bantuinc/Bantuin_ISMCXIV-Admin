using Bantuin_ISMCXIV_Admin.Data;
using Bantuin_ISMCXIV_Admin.Interfaces;
using Bantuin_ISMCXIV_Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bantuin_ISMCXIV_Admin.Repository
{
	public class RefreshTokenRepository : IRefreshTokenRepository
	{
		private readonly DataContext _context;
		private readonly UserManager<User> _userManager;

		public RefreshTokenRepository(DataContext context,
			UserManager<User> userManager)
        {
			_context = context;
			_userManager = userManager;
		}
        public async Task<RefreshToken> CreateRefreshTokenAsync(User user)
		{
			var refreshToken = new RefreshToken
			{
				Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
				Expired = DateTime.UtcNow.AddDays(7),
				Invalidated = false,
				User = user
			
			};
			_context.RefreshTokens.Add(refreshToken);
			await _context.SaveChangesAsync();

			return refreshToken;
		}

		public async Task<RefreshToken> GetRefreshTokenAsync(string token)
		{
			return await _context.RefreshTokens
				.Include(rt => rt.User)
				.SingleOrDefaultAsync(rt => rt.Token == token);
		}

		public async Task<bool> RevokeRefreshTokenAsync(string token)
		{
			var refreshToken = await GetRefreshTokenAsync(token);

			if (refreshToken != null)
			{
				refreshToken.Invalidated = true;
				await _context.SaveChangesAsync();
				return true;
			}

			return false;
		}
	}
}
