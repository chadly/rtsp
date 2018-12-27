using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Rtsp.Streaming.Video
{
	public class StreamCleaningHost : IHostedService
	{
		const int CleanupIntervalInSeconds = 90;

		readonly VideoConverter videos;
		readonly ILogger log;
		Timer timer;

		public StreamCleaningHost(VideoConverter videos, ILogger<StreamCleaningHost> log)
		{
			this.videos = videos;
			this.log = log;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			log.LogInformation("Stream cleaning service is starting.");

			timer = new Timer(Cleanup, null, TimeSpan.Zero, TimeSpan.FromSeconds(CleanupIntervalInSeconds));

			return Task.CompletedTask;
		}

		void Cleanup(object state)
		{
			log.LogInformation("Checking for streams to cleanup.");
			videos.PurgeUnusedStreams();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			log.LogInformation("Steam cleaning service is stopping.");

			timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			timer?.Dispose();
		}
	}
}
