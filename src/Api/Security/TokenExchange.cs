namespace Rtsp.Security
{
	public class TokenExchange
	{
		public string client_id { get; set; }
		public string client_secret { get; set; }

		public TokenGrantType grant_type { get; set; }

		public string code { get; set; }
		public string refresh_token { get; set; }
	}
}
