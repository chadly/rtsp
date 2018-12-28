using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;

namespace Rtsp.Streaming.Video
{
	public class VideoConverter
	{
		static readonly TimeSpan oldStreamThreshold = TimeSpan.FromMinutes(10);

		readonly AsyncReaderWriterLock lck = new AsyncReaderWriterLock();
		readonly Dictionary<int, Process> processes = new Dictionary<int, Process>();
		readonly ConcurrentDictionary<int, DateTime> accessTimes = new ConcurrentDictionary<int, DateTime>();
		readonly StreamingOptions opts;
		readonly ILogger log;

		public VideoConverter(IOptions<StreamingOptions> opts, ILogger<VideoConverter> log)
		{
			this.opts = opts.Value;
			this.log = log;
		}

		public async Task<string> StartVideoConversionAsync(int cameraId, string rtspUrl)
		{
			string camDir = Path.Combine(opts.OutputPath, cameraId.ToString());
			string masterPath = Path.Combine(camDir, "master.m3u8");
			string playlistPath = Path.Combine(camDir, "index.m3u8");

			using (await lck.ReaderLockAsync())
			{
				if (processes.ContainsKey(cameraId))
					return masterPath;
			}

			using (await lck.WriterLockAsync())
			{
				if (processes.ContainsKey(cameraId))
					return masterPath;

				log.LogInformation("Starting camera stream for {cameraId}", cameraId);

				EnsureEmptyDirectory(camDir);
				await WriteMasterPlaylistAsync(masterPath);

				var process = new Process
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = "ffmpeg",
						Arguments = $"-rtsp_transport tcp -i \"{rtspUrl}\" " +
							$"-an -vcodec copy " +
							$"-hls_time 1 -hls_list_size 5 -hls_flags delete_segments -hls_delete_threshold 10 " +
							$"-flags -global_header " +
							$"{playlistPath}"
					}
				};

				process.Start();

				processes.Add(cameraId, process);
				accessTimes.TryAdd(cameraId, DateTime.Now);

				while (!File.Exists(playlistPath))
					await Task.Delay(500);
			}

			return masterPath;
		}

		public void RecordCameraAccess(int cameraId)
		{
			log.LogDebug("Recording camera access for {cameraId}", cameraId);
			accessTimes[cameraId] = DateTime.Now;
		}

		public void PurgeUnusedStreams()
		{
			var camsToKill = new List<int>();

			using (lck.ReaderLock())
			{
				foreach (int cameraId in processes.Keys)
				{
					accessTimes.TryGetValue(cameraId, out DateTime lastAccessTime);
					DateTime threshold = lastAccessTime.Add(oldStreamThreshold);

					if (DateTime.Now > threshold)
						camsToKill.Add(cameraId);
				}
			}

			if (camsToKill.Count > 0)
			{
				log.LogInformation("Purging {count} unused camera streams", camsToKill.Count);

				using (lck.WriterLock())
				{
					foreach (int cameraId in camsToKill)
					{
						if (processes.TryGetValue(cameraId, out Process process))
						{
							log.LogDebug("Purging {cameraId} camera stream", cameraId);
							process.Kill();
						}

						processes.Remove(cameraId);
						accessTimes.Remove(cameraId, out _);
					}
				}
			}
		}

		static void EnsureEmptyDirectory(string path)
		{
			Directory.CreateDirectory(path);
			var existingFiles = Directory.GetFiles(path);

			foreach (var file in existingFiles)
			{
				File.Delete(file);
			}
		}

		static async Task WriteMasterPlaylistAsync(string path)
		{
			using (var file = File.Open(path, FileMode.CreateNew))
			{
				using (var writer = new StreamWriter(file))
				{
					await writer.WriteAsync("#EXTM3U\n");
					await writer.WriteAsync("#EXT-X-STREAM-INF:PROGRAM-ID=1, BANDWIDTH=2900000,CODECS=\"avc1.640029\"\n");
					await writer.WriteAsync("index.m3u8\n");

					await writer.FlushAsync();
				}
			}
		}
	}
}
