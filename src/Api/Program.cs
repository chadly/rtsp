using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Rtsp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
#if !DEBUG
				.ConfigureLogging(log => log.AddAzureWebAppDiagnostics())
#endif
				.UseStartup<Startup>();
		}
	}
}
