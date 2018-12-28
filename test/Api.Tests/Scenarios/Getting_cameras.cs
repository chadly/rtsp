using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Rtsp.Tests.Scenarios
{
	public class Getting_cameras : IntegrationTest
	{
		public Getting_cameras(DatabaseFixture fixture)
			   : base(fixture) { }

		public override async Task InitializeAsync()
		{
			await base.InitializeAsync();

			await api.AddCamera("Front Yard", "rtsp://example.com/front");
			await api.AddCamera("Backyard", "rtsp://example.com/back");
		}

		[Fact]
		public async Task Get_cameras()
		{
			var result = await api.GetCameras();

			response.StatusCode.Should().Be(HttpStatusCode.OK);

			result.Should().NotBeNull();
			result.Should().HaveCount(2);

			result.Should().BeEquivalentTo(new[]
			{
				new Camera
				{
					Name = "Front Yard",
					RtspUrl = "rtsp://example.com/front"
				},
				new Camera
				{
					Name = "Backyard",
					RtspUrl = "rtsp://example.com/back"
				}
			}, opts => opts.Including(c => c.Name).Including(c => c.RtspUrl).WithStrictOrdering());
		}
	}
}
