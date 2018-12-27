using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Rtsp.Google
{
	[ApiController]
	[Route("google")]
	public class GoogleHomeController : ControllerBase
	{
		readonly IDbConnection db;
		readonly ILogger<GoogleHomeController> log;

		const string AgentUserId = "69420";

		public GoogleHomeController(IDbConnection db, ILogger<GoogleHomeController> log)
		{
			this.db = db;
			this.log = log;
		}

		/// <remarks>
		/// https://developers.google.com/actions/smarthome/guides/camera
		/// </remarks>
		[HttpPost]
		[GoogleAction("action.devices.SYNC")]
		public ActionResult Sync(SyncAction action)
		{
			log.LogDebug("Receiving sync action {requestId}", action.RequestId);

			throw new NotImplementedException();
			//var cameras = opts.Cameras.Select(c => new
			//{
			//	c.Id,
			//	Type = "action.devices.types.CAMERA",
			//	Traits = new string[] { "action.devices.traits.CameraStream" },

			//	Name = new { c.Name },
			//	WillReportState = false,

			//	Attributes = new
			//	{
			//		CameraStreamSupportedProtocols = new string[] { "hls" },
			//		CameraStreamNeedAuthToken = false,
			//		CameraStreamNeedDrmEncryption = false
			//	}
			//});

			//return Ok(new
			//{
			//	action.RequestId,
			//	Payload = new
			//	{
			//		AgentUserId,
			//		Devices = cameras
			//	}
			//});
		}

		/// <remarks>
		/// https://developers.google.com/actions/smarthome/traits/camerastream
		/// </remarks>
		[HttpPost]
		[GoogleAction("action.devices.EXECUTE")]
		public ActionResult GetCameraStream(ExecuteAction action)
		{
			log.LogDebug("Receiving execute action {requestId}", action.RequestId);

			string cmd = action?.Inputs?.FirstOrDefault()?.Payload?.Commands?.FirstOrDefault()?.Execution?.FirstOrDefault()?.Command;
			if (!"action.devices.commands.GetCameraStream".Equals(cmd, StringComparison.CurrentCultureIgnoreCase))
				return NotFound();

			string deviceId = action?.Inputs?.FirstOrDefault()?.Payload?.Commands?.FirstOrDefault()?.Devices?.FirstOrDefault().Id;

			throw new NotImplementedException();
			//var cam = opts.Cameras.FirstOrDefault(c => c.Id == deviceId);
			//if (cam == null)
			//	return NotFound();

			//return Ok(new
			//{
			//	action.RequestId,
			//	Payload = new
			//	{
			//		Commands = new[]
			//		{
			//			new
			//			{
			//				Ids = new[] { deviceId },
			//				Status = "SUCCESS",
			//				States = new
			//				{
			//					CameraStreamAccessUrl=$"{opts.LocalNetworkUrl}{cam.Name}/master.m3u8"
			//				}
			//			}
			//		}
			//	}
			//});
		}

		/// <remarks>
		/// https://developers.google.com/actions/smarthome/create
		/// </remarks>
		[HttpPost]
		[GoogleAction("action.devices.QUERY")]
		public ActionResult QueryDevices(QueryAction action)
		{
			log.LogDebug("Receiving query action {requestId}", action.RequestId);

			throw new NotImplementedException();
			//var devices = action.Inputs.SelectMany(i => i.Payload.Devices).Aggregate(new Dictionary<string, QueryDeviceResult>(), (dic, d) =>
			//{
			//	if (opts.Cameras.Any(c => c.Id == d.Id))
			//		dic.Add(d.Id, new QueryDeviceResult { Online = true });

			//	return dic;
			//});

			//return Ok(new
			//{
			//	action.RequestId,
			//	Payload = new { devices }
			//});
		}
	}
}
