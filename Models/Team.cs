using System.ComponentModel.DataAnnotations.Schema;

namespace Bantuin_ISMCXIV_Admin.Models
{
	public class Team
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }
		public string? Name { get; set; }
		public string? University { get; set; }
		public string? StreetAddress { get; set; }
		public string? City { get; set; }
		public string? State { get; set; }
		public string? Country { get; set; }
		public string? PostalCode { get; set; }
		public Event? Event { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public ICollection<TeamMember>? TeamMembers { get; set; }

	}
}
