using System.Collections.Generic;

namespace Rtsp.Google
{
	public class QueryAction
	{
		public string RequestId { get; set; }
		public IEnumerable<QueryIntent> Inputs { get; set; }

		public class QueryIntent
		{
			public QueryPayload Payload { get; set; }
		}

		public class QueryPayload
		{
			public IEnumerable<Device> Devices { get; set; }
		}

		public class Device
		{
			public string Id { get; set; }
		}
	}
}