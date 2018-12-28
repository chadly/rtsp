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
		[Route("{cameraId:int}/master.m3u8")]
		public async Task<ActionResult> GetMasterPlaylist(int cameraId)
		{
			var camera = await api.GetCamera(cameraId);

			if (camera == null)
				return NotFound();

			string path = await videos.StartVideoConversionAsync(camera.Id, camera.RtspUrl);
			return File(System.IO.File.OpenRead(path), "application/vnd.apple.mpegurl");
		}

		[HttpGet]
		[Route("{cameraId:int}/index.m3u8")]
		public ActionResult GetPlaylist(int cameraId)
		{
			videos.RecordCameraAccess(cameraId);

			string path = Path.Combine(opts.OutputPath, cameraId.ToString(), "index.m3u8");
			return File(System.IO.File.OpenRead(path), "application/vnd.apple.mpegurl");
		}

		[HttpGet]
		[Route("{cameraId:int}/{name}.ts")]
		public ActionResult GetParts(int cameraId, string name)
		{
			videos.RecordCameraAccess(cameraId);

			string path = Path.Combine(opts.OutputPath, cameraId.ToString(), $"{name}.ts");
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
