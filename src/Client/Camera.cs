using System.Collections.Generic;

namespace Rtsp
{
	public class Camera
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public IEnumerable<string> NickNames { get; set; }

		public string RtspUrl { get; set; }
	}
}
