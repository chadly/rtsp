using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Rtsp.Tests.Scenarios
{
	public class Getting_a_camera : IntegrationTest
	{
		int cameraId;

		public Getting_a_camera(DatabaseFixture fixture)
			   : base(fixture) { }

		public override async Task InitializeAsync()
		{
			await base.InitializeAsync();

			cameraId = (await api.AddCamera("Front Yard", "rtsp://example.com/front")).Id;
		}

		[Fact]
		public async Task Get_camera()
		{
			var result = await api.GetCamera(cameraId);

			response.StatusCode.Should().Be(HttpStatusCode.OK);

			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(new Camera
			{
				Name = "Front Yard",
				RtspUrl = "rtsp://example.com/front"
			}, opts => opts.Including(c => c.Name).Including(c => c.RtspUrl));
		}

		[Fact]
		public async Task Get_camera_that_does_not_exist()
		{
			var result = await api.GetCamera(48482387);

			response.StatusCode.Should().Be(HttpStatusCode.NotFound);

			result.Should().BeNull();
		}
	}
}
