using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalROnline.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SignalROnline.Controllers
{
	public class ProfilesController : Controller
	{
		private readonly IConfiguration _configuration;
		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public ProfilesController(IHttpContextAccessor httpContextAccessor,IConfiguration configuration, ApplicationDbContext context)
		{
			_httpContextAccessor = httpContextAccessor;
			_context = context;
			_configuration = configuration;
		}
		[HttpGet("checkprofile")]
		public async Task <ActionResult<bool>> CheckProfile(string username)
		{
			var profile =  _context.Users.FirstOrDefault(p => p.Username == username);

			if (profile == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		[HttpGet("getallusers")]
		public async Task<ActionResult<List<string>>> GetAllUsers()
		{
			List<User> userlist = await _context.Users.ToListAsync();
			List<string> nicknamelist = userlist.Select(user => user.Nickname).ToList();
			return nicknamelist;
		}


		[HttpPost("register")]
		public async Task<ActionResult<User>> Register(string nickname, Icon icon)
		{
			if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) &&
	authorizationHeader.FirstOrDefault() is string accestoken)
			{
				var httpContext = _httpContextAccessor.HttpContext;
				if (httpContext == null)
				{
					// Handle the case where the HttpContext is not available
					return StatusCode(500, "HttpContext not available");
				}

				var token = accestoken.ToString().Replace("Bearer ", "");

				var handler = new JwtSecurityTokenHandler();

				var jwtToken = handler.ReadJwtToken(token);

				// Get the Name claim from the token
				var usernameClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
				string username = usernameClaim?.Value;
				if (username != null)
				{

					var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
					if (existingUser != null)
					{
						return BadRequest("Username is taken.");
					}
					User newUser = new User
					{

						Username = username,
						Nickname = nickname,
						Icon = icon,
						Level = 1,
					};


					_context.Users.Add(newUser);
					await _context.SaveChangesAsync();
					return Ok(newUser);
				}
				else return BadRequest("user taken");
			}
			else return BadRequest("error");
		}

	}
}
