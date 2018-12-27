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
		readonly Dictionary<string, Process> processes = new Dictionary<string, Process>();
		readonly ConcurrentDictionary<string, DateTime> accessTimes = new ConcurrentDictionary<string, DateTime>();
		readonly StreamingOptions opts;
		readonly ILogger log;

		public VideoConverter(IOptions<StreamingOptions> opts, ILogger<VideoConverter> log)
		{
			this.opts = opts.Value;
			this.log = log;
		}

		public async Task<string> StartVideoConversionAsync(string cam, string rtspUrl)
		{
			string camDir = Path.Combine(opts.OutputPath, cam);
			string masterPath = Path.Combine(camDir, "master.m3u8");
			string playlistPath = Path.Combine(camDir, "index.m3u8");

			using (await lck.ReaderLockAsync())
			{
				if (processes.ContainsKey(cam))
					return masterPath;
			}

			using (await lck.WriterLockAsync())
			{
				if (processes.ContainsKey(cam))
					return masterPath;

				log.LogInformation("Starting camera stream for {cam}", cam);

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

				processes.Add(cam, process);
				accessTimes.TryAdd(cam, DateTime.Now);

				while (!File.Exists(playlistPath))
					await Task.Delay(500);
			}

			return masterPath;
		}

		public void RecordCameraAccess(string cam)
		{
			log.LogDebug("Recording camera access for {cam}", cam);
			accessTimes[cam] = DateTime.Now;
		}

		public void PurgeUnusedStreams()
		{
			var camsToKill = new List<string>();

			using (lck.ReaderLock())
			{
				foreach (string cam in processes.Keys)
				{
					accessTimes.TryGetValue(cam, out DateTime lastAccessTime);
					DateTime threshold = lastAccessTime.Add(oldStreamThreshold);

					if (DateTime.Now > threshold)
						camsToKill.Add(cam);
				}
			}

			if (camsToKill.Count > 0)
			{
				log.LogInformation("Purging {count} unused camera streams", camsToKill.Count);

				using (lck.WriterLock())
				{
					foreach (string cam in camsToKill)
					{
						if (processes.TryGetValue(cam, out Process process))
						{
							log.LogDebug("Purging {cam} camera stream", cam);
							process.Kill();
						}

						processes.Remove(cam);
						accessTimes.Remove(cam, out _);
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
