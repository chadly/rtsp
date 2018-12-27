using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Rtsp.Tests.Scenarios
{
	public class Getting_a_camera : IntegrationTest
	{
		public Getting_a_camera(DatabaseFixture fixture)
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
		}

		[Fact]
		public async Task Get_camera()
		{
			var result = await api.GetCamera("cam1");

			response.StatusCode.Should().Be(HttpStatusCode.OK);

			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(new Camera
			{
				Id = "cam1",
				Name = "Front Yard",
				NickNames = new string[0],
				RtspUrl = "rtsp://example.com/front"
			});
		}

		[Fact]
		public async Task Get_camera_that_does_not_exist()
		{
			var result = await api.GetCamera("cam2");

			response.StatusCode.Should().Be(HttpStatusCode.NotFound);

			result.Should().BeNull();
		}
	}
}
