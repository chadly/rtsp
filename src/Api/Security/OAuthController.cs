using System;
using Microsoft.AspNetCore.Mvc;

namespace Rtsp.Security
{
	/// <remarks>
	/// https://developers.google.com/actions/identity/oauth2?oauth=code
	/// </remarks>
	[ApiController]
	public class OAuthController : ControllerBase
	{
		[HttpGet]
		[Route("auth")]
		public ActionResult Authorize(string redirect_uri, string state)
		{
			Guid token = Guid.NewGuid();
			return Redirect($"{redirect_uri}?code={token}&state={state}");
		}

		[HttpPost]
		[Route("token")]
		public ActionResult GenerateAccessToken([FromForm] TokenExchange data)
		{
			if (data.grant_type == TokenGrantType.authorization_code)
			{
				return Ok(new
				{
					token_type = "Bearer",
					access_token = Guid.NewGuid(),
					refresh_token = Guid.NewGuid(),
					expires_in = 3600
				});
			}

			if (data.grant_type == TokenGrantType.refresh_token)
			{
				return Ok(new
				{
					token_type = "Bearer",
					access_token = Guid.NewGuid(),
					expires_in = 3600
				});
			}

			return BadRequest(new { error = "invalid_grant" });
		}
	}
}
