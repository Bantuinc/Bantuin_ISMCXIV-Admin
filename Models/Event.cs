using System.ComponentModel.DataAnnotations.Schema;

namespace Bantuin_ISMCXIV_Admin.Models
{
	public class Event
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public string? Picture { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public ICollection<Team>? Teams { get; set; }
	}
}
