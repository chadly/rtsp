using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Rtsp.Cameras
{
	public class NewCamera
	{
		[Required]
		public string Id { get; set; }

		[Required]
		public string Name { get; set; }

		public IEnumerable<string> NickNames { get; set; }
		public string NickNamesJson => NickNames == null ? null : JsonConvert.SerializeObject(NickNames);

		[Required]
		public string RtspUrl { get; set; }
	}
}
