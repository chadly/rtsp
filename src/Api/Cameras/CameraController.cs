using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Archon.Data;
using Microsoft.AspNetCore.Mvc;

namespace Rtsp.Cameras
{
	[ApiController]
	[Route("cameras")]
	public class CameraController : ControllerBase
	{
		readonly IDbConnection db;

		public CameraController(IDbConnection db)
		{
			this.db = db;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Camera>>> GetCameras()
		{
			var cameras = await db.SelectCameras();
			return Ok(cameras);
		}

		[HttpGet]
		[Route("{id:int}")]
		public async Task<ActionResult<Camera>> GetCameraById(int id)
		{
			var camera = await db.SelectCamera(id);

			if (camera == null)
				return NotFound();

			return camera;
		}

		[HttpPost]
		public async Task<ActionResult<Camera>> AddCamera(NewCamera camera)
		{
			int cameraId;
			Camera result;

			using (var tx = db.BeginTransaction())
			{
				try
				{
					cameraId = await db.InsertCamera(camera);
				}
				catch (SqlException ex) when (ex.IsDuplicateKeyException())
				{
					return Conflict(new { Message = $"A camera with ID '' already exists." });
				}

				result = await db.SelectCamera(cameraId);

				tx.Commit();
			}

			return result;
		}

		[HttpDelete]
		[Route("{id:int}")]
		public async Task<ActionResult> RemoveCamera(int id)
		{
			int count = await db.DeleteCamera(id);

			if (count > 0)
				return NoContent();

			return NotFound();
		}
	}
}
