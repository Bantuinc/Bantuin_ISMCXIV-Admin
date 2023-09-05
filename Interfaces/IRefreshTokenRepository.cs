using Bantuin_ISMCXIV_Admin.Models;

namespace Bantuin_ISMCXIV_Admin.Interfaces
{
	public interface IRefreshTokenRepository
	{
		Task<RefreshToken> CreateRefreshTokenAsync(User user);
		Task<RefreshToken> GetRefreshTokenAsync(string token);
		Task<bool> RevokeRefreshTokenAsync(string token);
	}
}
