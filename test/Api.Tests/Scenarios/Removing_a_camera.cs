using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Rtsp.Tests.Scenarios
{
	public class Removing_a_camera : IntegrationTest
	{
		int camera1Id, camera2Id;

		public Removing_a_camera(DatabaseFixture fixture)
			   : base(fixture) { }

		public override async Task InitializeAsync()
		{
			await base.InitializeAsync();

			camera1Id = (await api.AddCamera("Front Yard", "rtsp://example.com/front")).Id;
			camera2Id = (await api.AddCamera("Backyard", "rtsp://example.com/back")).Id;
		}

		[Fact]
		public async Task Remove_a_camera()
		{
			await api.RemoveCamera(camera1Id);
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);

			var cameras = await api.GetCameras();
			cameras.Should().HaveCount(1);
			cameras.Select(c => c.Id).Should().NotContain(camera1Id);
		}

		[Fact]
		public async Task Remove_a_camera_that_does_not_exist()
		{
			await api.RemoveCamera(23948723);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}
	}
}
