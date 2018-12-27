using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Rtsp.Streaming.Video
{
	[ApiController]
	public class VideoController : ControllerBase
	{
		readonly VideoConverter videos;
		readonly IRtspService api;
		readonly StreamingOptions opts;

		public VideoController(VideoConverter videos, IRtspService api, IOptions<StreamingOptions> opts)
		{
			this.videos = videos;
			this.api = api;
			this.opts = opts.Value;
		}

		[HttpGet]
		[Route("{cam}/master.m3u8")]
		public async Task<ActionResult> GetMasterPlaylist(string cam)
		{
			var camera = await api.GetCamera(cam);

			if (camera == null)
				return NotFound();

			string path = await videos.StartVideoConversionAsync(camera.Id, camera.RtspUrl);
			return File(System.IO.File.OpenRead(path), "application/vnd.apple.mpegurl");
		}

		[HttpGet]
		[Route("{cam}/index.m3u8")]
		public ActionResult GetPlaylist(string cam)
		{
			videos.RecordCameraAccess(cam);

			string path = Path.Combine(opts.OutputPath, cam, "index.m3u8");
			return File(System.IO.File.OpenRead(path), "application/vnd.apple.mpegurl");
		}

		[HttpGet]
		[Route("{cam}/{name}.ts")]
		public ActionResult GetParts(string cam, string name)
		{
			videos.RecordCameraAccess(cam);

			string path = Path.Combine(opts.OutputPath, cam, $"{name}.ts");
			try
			{
				return File(System.IO.File.OpenRead(path), "video/mp2t");
			}
			catch (FileNotFoundException)
			{
				return NotFound();
			}
		}
	}
}
