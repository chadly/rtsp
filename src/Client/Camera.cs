using System;

namespace Rtsp
{
	public class Camera
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string RtspUrl { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
