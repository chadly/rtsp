using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Rtsp.Tests.Scenarios
{
	public class Adding_a_camera : IntegrationTest
	{
		public Adding_a_camera(DatabaseFixture fixture)
			: base(fixture) { }

		[Fact]
		public async Task Add_camera_with_all_fields()
		{
			var result = await api.AddCamera(new Camera
			{
				Id = "cam1",
				Name = "Front Yard",
				NickNames = new[] { "front", "the yard" },
				RtspUrl = "rtsp://example.com"
			});

			response.StatusCode.Should().Be(HttpStatusCode.OK);

			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(new Camera
			{
				Id = "cam1",
				Name = "Front Yard",
				NickNames = new[] { "front", "the yard" },
				RtspUrl = "rtsp://example.com"
			});
		}

		[Fact]
		public async Task Add_camera_with_only_required_fields()
		{
			var result = await api.AddCamera(new Camera
			{
				Id = "cam1",
				Name = "Front Yard",
				RtspUrl = "rtsp://example.com"
			});

			response.StatusCode.Should().Be(HttpStatusCode.OK);

			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(new Camera
			{
				Id = "cam1",
				Name = "Front Yard",
				NickNames = new string[0],
				RtspUrl = "rtsp://example.com"
			});
		}

		[Fact]
		public async Task Add_camera_without_id()
		{
			await Assert.ThrowsAsync<HttpRequestException>(() => api.AddCamera(new Camera
			{
				Name = "Front Yard",
				RtspUrl = "rtsp://example.com"
			}));

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Add_camera_without_name()
		{
			await Assert.ThrowsAsync<HttpRequestException>(() => api.AddCamera(new Camera
			{
				Id = "cam1",
				RtspUrl = "rtsp://example.com"
			}));

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Add_camera_without_rtsp_url()
		{
			await Assert.ThrowsAsync<HttpRequestException>(() => api.AddCamera(new Camera
			{
				Id = "cam1",
				Name = "Front Yard"
			}));

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Add_camera_with_duplicate_id()
		{
			await api.AddCamera(new Camera
			{
				Id = "cam1",
				Name = "Front Yard",
				RtspUrl = "rtsp://example.com"
			});

			await Assert.ThrowsAsync<HttpRequestException>(() => api.AddCamera(new Camera
			{
				Id = "cam1",
				Name = "Backyard",
				RtspUrl = "rtsp://example.com/backyard"
			}));

			response.StatusCode.Should().Be(HttpStatusCode.Conflict);
		}

		[Fact]
		public async Task Add_camera_with_duplicate_name()
		{
			await api.AddCamera(new Camera
			{
				Id = "cam1",
				Name = "Front Yard",
				RtspUrl = "rtsp://example.com"
			});

			await api.AddCamera(new Camera
			{
				Id = "cam2",
				Name = "Front Yard",
				RtspUrl = "rtsp://example.com/front2"
			});

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}
	}
}
