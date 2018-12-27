using System.Data;
using System.Data.SqlClient;
using Archon.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rtsp.Tests
{
	public class TestStartup : Startup
	{
		public TestStartup(IConfiguration config)
			: base(config) { }

		protected override void ConfigureOverridableServices(IServiceCollection services)
		{
			services.AddScoped<IDbConnection>(s => new DataContext(new SqlConnection(s.GetRequiredService<Database>().ConnectionString)));
		}
	}
}
