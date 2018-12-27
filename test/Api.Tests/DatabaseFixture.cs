using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Archon.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Rtsp.Tests
{
	public class DatabaseFixture : IAsyncLifetime
	{
		private const int CommandTimeout = 120;

		private readonly IMessageSink log;
		private IConfigurationRoot config;
		private Database db;

		public string DatabaseSuffix { get; set; }

		public DatabaseFixture(IMessageSink log)
		{
			this.log = log;
		}

		public async Task InitializeAsync()
		{
			config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("test-settings.json")
				.Build();

			var conStr = new SqlConnectionStringBuilder(config["db"]);
			conStr.InitialCatalog += DatabaseSuffix;

			log.OnMessage(new DiagnosticMessage("Connection string set to '{0}'", conStr.ToString()));
			db = new Database(conStr.ToString(), typeof(Program), CommandTimeout);

			log.OnMessage(new DiagnosticMessage("Initializing database..."));

			await db.RebuildAsync();
		}

		public Task DisposeAsync() => Task.CompletedTask;

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(db);
			services.AddSingleton<IConfiguration>(config);
		}

		public async Task<IDbConnection> OpenConnectionAsync()
		{
			var con = new DataContext(new SqlConnection(db.ConnectionString));
			await con.OpenAsync();

			return con;
		}

		public Task ClearAsync() => db.ClearAsync();
	}
}
