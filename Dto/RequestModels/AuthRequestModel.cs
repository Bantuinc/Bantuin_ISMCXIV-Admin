using System.ComponentModel.DataAnnotations;

namespace Bantuin_ISMCXIV_Admin.Dto.RequestModels
{
	public class RegisterModel
	{
		[Required]
		[StringLength(100)]
		public string? Name { get; set; }
		[Required]
		[StringLength(50)]
		public string? UserName { get; set; }
		[Required]
		[StringLength(100)]
		[EmailAddress]
		public string? Email { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 6)]
		public string? Password { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 6)]
		public string? ConfirmPassword { get; set; }

	}

	public class LoginModel
	{
		[Required]
		[StringLength(100)]
		public string? Email { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 6)]
		public string? Password { get; set; }
	}
}
