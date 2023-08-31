using System.Collections.Concurrent; // Import ConcurrentDictionary
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

using Microsoft.OpenApi.Models;
using SignalROnline;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SignalROnline
{

	public class OnlineHub : Hub
	{
		static ConcurrentDictionary<string, string> connectedUsers = new ConcurrentDictionary<string, string>();
		

		
		public async override Task OnConnectedAsync()
		{

			string accessToken = Context.GetHttpContext().Request.Query["access_token"];
			var token = accessToken.ToString().Replace("Bearer ", "");

			var handler = new JwtSecurityTokenHandler();

			var jwtToken = handler.ReadJwtToken(token);

			// Get the Name claim from the token
			var usernameClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
			var useridClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
			string username = usernameClaim?.Value;
			int userid = int.Parse(useridClaim.Value);
			Context.Items["username"] = username;
			Context.Items["userid"] = userid;


			if (!connectedUsers.ContainsKey(username))
			{
				connectedUsers.TryAdd(username, username);
				await Clients.All.SendAsync("UsersList", connectedUsers.Values);
			}
			await Clients.All.SendAsync("UsersID", userid);
			await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
			await Clients.All.SendAsync("UsersList", connectedUsers.Values);
		}


		public override async Task OnDisconnectedAsync(Exception exception)
		{
			string myusername = Context.Items["username"].ToString(); 

			if (connectedUsers.ContainsKey(myusername))
			{
				connectedUsers.TryRemove(myusername, out _);
				await Clients.All.SendAsync("UsersList", connectedUsers.Values);
			}

			await base.OnDisconnectedAsync(exception);
		}


	}
}