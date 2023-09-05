using Bantuin_ISMCXIV_Admin.Dto.RequestModels;
using Bantuin_ISMCXIV_Admin.Helpers;
using Bantuin_ISMCXIV_Admin.Interfaces;
using Bantuin_ISMCXIV_Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bantuin_ISMCXIV_Admin.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : Controller
	{
		private readonly ResponseHelper _responseHelper;
		private readonly UserManager<User> _userManager;
		private readonly IConfiguration _configuration;
		private readonly IRefreshTokenRepository _refreshTokenRepository;

		public AuthController(ResponseHelper responseHelper, 
			UserManager<User> userManager, 
			IConfiguration configuration,
			IRefreshTokenRepository refreshTokenRepository)
		{
			_responseHelper = responseHelper;
			_userManager = userManager;
			_configuration = configuration;
			_refreshTokenRepository = refreshTokenRepository;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> RegisterUser([FromBody] RegisterModel registerModel)
		{
			try
			{
				if (registerModel == null)
					return BadRequest(_responseHelper.Error("Invalid payload"));

				if (registerModel.Password != registerModel.ConfirmPassword)
					return BadRequest(_responseHelper.Error("Password and Confirm Password do not match"));

				var userAdd = new User
				{
					FullName = registerModel.Name,
					UserName = registerModel.UserName,
					Email = registerModel.Email,
				};

				var result = await _userManager.CreateAsync(userAdd, registerModel.Password!);
				if (result.Succeeded)
					await _userManager.AddToRoleAsync(userAdd, "User");
				return Ok(_responseHelper.Success("User created successfully"));
			}
			catch (SqlException ex)
			{
				return StatusCode(500, _responseHelper.Error("Something went wrong in sql execution", 500, ex.Message));
			}
			catch (Exception ex)
			{
				return StatusCode(500, _responseHelper.Error("Something went wrong", 500, ex.Message));
			}
		}

		[HttpPost("Login")]
		[ProducesResponseType(204)]
		public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
		{
			try
			{
				if (loginModel == null)
					return BadRequest(_responseHelper.Error("Invalid payload"));

				var user = await _userManager.FindByNameAsync(loginModel.Email!);
				if (user == null)
				{
					user = await _userManager.FindByEmailAsync(loginModel.Email!);
					if (user == null)
						return BadRequest(_responseHelper.Error("User not found"));
				}

				var result = await _userManager.CheckPasswordAsync(user, loginModel.Password!);
				if (!result)
					return BadRequest(_responseHelper.Error("Invalid password"));

				var tokenString = CreateAccessToken(user);

				var refreshToken = await _refreshTokenRepository.CreateRefreshTokenAsync(user);
				SetRefreshToken(refreshToken);

				return Ok(_responseHelper.Success("", new { token = tokenString.Result, expired = DateTime.Now.AddMinutes(30) }));
			}
			catch (SqlException ex)
			{
				return StatusCode(500, _responseHelper.Error("Something went wrong in sql execution", 500, ex.Message));
			}
			catch (Exception ex)
			{
				return StatusCode(500, _responseHelper.Error("Something went wrong", 500, ex.Message));
			}
		}

		[HttpGet("exchange-token")]
		public async Task<IActionResult> ExchangeRefreshToken()
		{
			try
			{
				var refreshTokenCookie = Request.Cookies["refreshToken"];
				if (!string.IsNullOrEmpty(refreshTokenCookie))
				{
					var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshTokenCookie);
					if (refreshToken == null || refreshToken.Invalidated || refreshToken.Expired <= DateTime.UtcNow)
					{
						return BadRequest(_responseHelper.Error("Invalid refresh token."));
					}
					var newAccessToken = CreateAccessToken(refreshToken.User!);
					return Ok(_responseHelper.Success("", new { token = newAccessToken.Result, expired = DateTime.Now.AddMinutes(30) }));
				}

				return BadRequest(_responseHelper.Error("Invalid refresh token."));
			}
			catch (SqlException ex)
			{
				return StatusCode(500, _responseHelper.Error("Something went wrong in sql execution", 500, ex.Message));
			}
			catch (Exception ex)
			{
				return StatusCode(500, _responseHelper.Error("Something went wrong", 500, ex.Message));
			}
		}

		[HttpPatch("revoke-token")]
		public async Task<IActionResult> RevokeRefreshToken()
		{
			try
			{
				var refreshTokenCookie = Request.Cookies["refreshToken"];
				if (!string.IsNullOrEmpty(refreshTokenCookie))
				{
					var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshTokenCookie);
					if (refreshToken == null || refreshToken.Invalidated || refreshToken.Expired <= DateTime.UtcNow)
					{
						return BadRequest(_responseHelper.Error("Invalid refresh token."));
					}
					await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken.Token!);
					return Ok(_responseHelper.Success("Refresh token revoked."));
				}

				return BadRequest(_responseHelper.Error("Invalid refresh token."));
			}
			catch (SqlException ex)
			{
				return StatusCode(500, _responseHelper.Error("Something went wrong in sql execution", 500, ex.Message));
			}
			catch (Exception ex)
			{
				return StatusCode(500, _responseHelper.Error("Something went wrong", 500, ex.Message));
			}
		}

		private async Task<string> CreateAccessToken(User user)
		{
			var roles = await _userManager.GetRolesAsync(user);
			var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
			var tokenKey = Encoding.UTF8.GetBytes(_configuration.GetSection("Authentication:Schemes:Bearer:SigningKeys:0:Value").Value!);
			var tokenExpires = DateTime.Now.AddMinutes(30);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Audience = _configuration["Jwt:Audience"],
				Issuer = _configuration["Jwt:Issuer"],
				Subject = new ClaimsIdentity(new Claim[]
				{
						new Claim(ClaimTypes.NameIdentifier, user.Id),
						new Claim(ClaimTypes.Name, user.FullName!),
						new Claim(ClaimTypes.Role, roles.FirstOrDefault()!)
				}),
				Expires = tokenExpires,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
			};
			var securityToken = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
			var tokenString = jwtSecurityTokenHandler.WriteToken(securityToken);

			return tokenString;
		}
		
		private void SetRefreshToken(RefreshToken refreshToken)
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = DateTime.Now.AddDays(7)
			};
			Response.Cookies.Append("refreshToken", refreshToken.Token!, cookieOptions);
		}
	}
}
