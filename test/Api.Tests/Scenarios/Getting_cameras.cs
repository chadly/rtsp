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
					Id = "cam1",
					Name = "Front Yard",
					Nicknames = new string[0],
					RtspUrl = "rtsp://example.com/front"
				},
				new Camera
				{
					Id = "cam2",
					Name = "Backyard",
					Nicknames = new string[0],
					RtspUrl = "rtsp://example.com/back"
				}
			});
		}
	}
}
