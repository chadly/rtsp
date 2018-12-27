using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Rtsp.Tests.Scenarios
{
	public class Removing_a_camera : IntegrationTest
	{
		public Removing_a_camera(DatabaseFixture fixture)
			   : base(fixture) { }

		public override async Task InitializeAsync()
		{
			await base.InitializeAsync();

			await api.AddCamera(new Camera
			{
				Id = "cam1",
				Name = "Front Yard",
				RtspUrl = "rtsp://example.com/front"
			});

			await api.AddCamera(new Camera
			{
				Id = "cam2",
				Name = "Backyard",
				RtspUrl = "rtsp://example.com/back"
			});
		}

		[Fact]
		public async Task Remove_a_camera()
		{
			await api.RemoveCamera("cam1");
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);

			var cameras = await api.GetCameras();
			cameras.Should().HaveCount(1);
			cameras.Select(c => c.Id).Should().NotContain("cam1");
		}

		[Fact]
		public async Task Remove_a_camera_that_does_not_exist()
		{
			await api.RemoveCamera("cam69");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}
	}
}
