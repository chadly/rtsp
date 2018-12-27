using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rtsp.Cameras
{
	public class Camera
	{
		public string Id { get; set; }
		public string Name { get; set; }

		[JsonIgnore]
		public string NickNamesJson { get; set; }

		public IEnumerable<string> NickNames => String.IsNullOrWhiteSpace(NickNamesJson) ? new string[0] : JsonConvert.DeserializeObject<IEnumerable<string>>(NickNamesJson);

		public string RtspUrl { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
