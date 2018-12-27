using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Rtsp.Tests
{
	[Collection(DatabaseCollection.Name)]
	public abstract class IntegrationTest : IAsyncLifetime
	{
		readonly DatabaseFixture fixture;
		readonly TestServer server;

		protected IServiceProvider Services => server.Host.Services;

		protected readonly IRtspService api;

		protected HttpResponseMessage response;

		public IntegrationTest(DatabaseFixture fixture)
		{
			this.fixture = fixture;

			server = new TestServer(new WebHostBuilder()
				.ConfigureServices(services =>
				{
					fixture.ConfigureServices(services);

					services.AddSingleton<IRtspService>(s =>
					{
						var handler = server.CreateHandler();
						return new HttpRtspService(new HttpClient(new HttpResponseLogger(response =>
						{
							// keep track of the latest response so we can assert against it
							this.response = response;
						}, handler))
						{
							BaseAddress = new Uri("http://example.com/")
						});
					});
				})
				.UseStartup<TestStartup>()
			);

			api = Services.GetRequiredService<IRtspService>();
		}

		public virtual void Dispose()
		{
			server?.Dispose();
		}

		public virtual Task InitializeAsync() => fixture.ClearAsync(); //setup fresh database for test run
		public virtual Task DisposeAsync() => Task.CompletedTask;
	}
}