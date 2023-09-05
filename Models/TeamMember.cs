using Bantuin_ISMCXIV_Admin.Data.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bantuin_ISMCXIV_Admin.Models
{
	public class TeamMember
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }
		public string? Name { get; set; }
		public string? Nim { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? Ktm { get; set; }
		public Team? Team { get; set; }
		public User? User { get; set; }
		public string? InvitationToken { get; set; }
		public TeamMemberRole Role { get; set; }
		public TeamMemberStatus Status { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
