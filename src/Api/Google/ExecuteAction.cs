using System.Collections.Generic;

namespace Rtsp.Google
{
	public class ExecuteAction
	{
		public string RequestId { get; set; }
		public IEnumerable<ExecuteIntent> Inputs { get; set; }

		public class ExecuteIntent
		{
			public ExecutePayload Payload { get; set; }
		}

		public class ExecutePayload
		{
			public IEnumerable<ExecuteCommand> Commands { get; set; }
		}

		public class ExecuteCommand
		{
			public IEnumerable<Device> Devices { get; set; }
			public IEnumerable<ExecuteExecution> Execution { get; set; }
		}

		public class Device
		{
			public string Id { get; set; }
		}

		public class ExecuteExecution
		{
			public string Command { get; set; }
			public ExecutionParams Params { get; set; }
		}

		public class ExecutionParams
		{
			public bool StreamToChromecast { get; set; }
			public IEnumerable<string> SupportedStreamProtocols { get; set; }
		}
	}
}