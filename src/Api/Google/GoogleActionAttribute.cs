using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Rtsp.Google
{
	public class GoogleActionAttribute : Attribute, IActionConstraint
	{
		readonly string expectedIntent;
		public int Order => 10;

		public GoogleActionAttribute(string intent)
		{
			if (String.IsNullOrWhiteSpace(intent))
				throw new ArgumentNullException(nameof(intent));

			expectedIntent = intent;
		}

		public bool Accept(ActionConstraintContext context)
		{
			var httpContext = context.RouteContext.HttpContext;
			var req = httpContext.Request;
			var res = httpContext.Response;
			var log = httpContext.RequestServices.GetRequiredService<ILogger<GoogleActionAttribute>>();
			return Accept(log, req, res);
		}

		bool Accept(ILogger<GoogleActionAttribute> log, HttpRequest req, HttpResponse res)
		{
			req.Body = BufferStream(req.Body);
			var reader = new StreamReader(req.Body);

			string json = reader.ReadToEnd();
			req.Body.Position = 0;

			log.LogDebug("Checking json: {json}", json);

			if (String.IsNullOrWhiteSpace(json))
				return false;

			var action = JsonConvert.DeserializeObject<GoogleAction>(json);
			if (!action.Inputs.Any())
				return false;

			string actualIntent = action.Inputs.First().Name;
			if (String.IsNullOrWhiteSpace(actualIntent))
				return false;

			log.LogDebug("Comparing actual intent '{actualIntent}' with expected intent '{expectedIntent}'", actualIntent, expectedIntent);
			return expectedIntent.Equals(actualIntent, StringComparison.CurrentCultureIgnoreCase);
		}

		Stream BufferStream(Stream stream)
		{
			if (stream is MemoryStream)
				return stream;

			var mem = new MemoryStream();
			using (stream)
				stream.CopyTo(mem);

			mem.Position = 0;
			return mem;
		}

		class GoogleAction
		{
			public IEnumerable<Intent> Inputs { get; set; }

			public class Intent
			{
				[JsonProperty("intent")]
				public string Name { get; set; }
			}
		}
	}
}
