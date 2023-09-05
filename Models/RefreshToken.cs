using System.ComponentModel.DataAnnotations.Schema;

namespace Bantuin_ISMCXIV_Admin.Models
{
	public class RefreshToken
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }
		public string? Token { get; set; }
		public DateTime Expired { get; set; }
		public bool Invalidated { get; set; }
		public User? User { get; set; }
	}
}
