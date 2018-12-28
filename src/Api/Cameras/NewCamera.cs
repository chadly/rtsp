using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Rtsp.Cameras
{
	public class NewCamera
	{
		string name, rtspUrl;

		[Required]
		[MaxLength(50)]
		public string Name
		{
			get => name;
			set => name = value?.Trim();
		}

		[Required]
		[MaxLength(250)]
		public string RtspUrl
		{
			get => rtspUrl;
			set => rtspUrl = value?.Trim();
		}
	}
}
