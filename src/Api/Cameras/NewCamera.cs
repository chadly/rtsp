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

		public IEnumerable<string> Nicknames { get; set; }
		public string NicknamesJson => Nicknames == null ? null : JsonConvert.SerializeObject(Nicknames);

		[Required]
		public string RtspUrl { get; set; }
	}
}
