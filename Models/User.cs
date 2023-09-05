using Microsoft.AspNetCore.Identity;

namespace Bantuin_ISMCXIV_Admin.Models
{
	public class User : IdentityUser
	{
		public string? FullName { get; set; }
		public string? Phone { get; set; }
		public string? Picture { get; set; }
		public string? ActivationToken { get; set; }
		public string? RefreshToken { get; set; }
		public int Status { get; set; }
        public ICollection<TeamMember>? TeamMembers { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
    }
}
