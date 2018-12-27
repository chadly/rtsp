using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rtsp.Tests
{
	public class HttpResponseLogger : DelegatingHandler
	{
		readonly Action<HttpResponseMessage> onResponse;

		public HttpResponseLogger(Action<HttpResponseMessage> onResponse, HttpMessageHandler innerHandler)
			: base(innerHandler)
		{
			this.onResponse = onResponse;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var response = await base.SendAsync(request, cancellationToken);
			onResponse(response);
			return response;
		}
	}
}
