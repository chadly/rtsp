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
		public string NicknamesJson { get; set; }

		public IEnumerable<string> Nicknames => String.IsNullOrWhiteSpace(NicknamesJson) ? new string[0] : JsonConvert.DeserializeObject<IEnumerable<string>>(NicknamesJson);

		public string RtspUrl { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
