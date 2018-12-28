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
			var result = await api.AddCamera(" Front Yard  ", " rtsp://example.com  ");

			response.StatusCode.Should().Be(HttpStatusCode.OK);

			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(new Camera
			{
				Name = "Front Yard",
				RtspUrl = "rtsp://example.com"
			}, opts => opts.Including(c => c.Name).Including(c => c.RtspUrl));
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		[InlineData("This is a really long name that is more than max length for the field")]
		public async Task Add_camera_with_invalid_name(string name)
		{
			await Assert.ThrowsAsync<HttpRequestException>(() => api.AddCamera(name, "rtsp://example.com"));
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		[InlineData("rtsp://thisisareallylongrtspurlthatislongerthanthemaxlengthforthefield/something.still/needtogolongerbecauseweactuallyallowaprettylengthy/characterlimitforthisfield/blablablablablablablablablablablablablablablablablablablablablablablablablablablablablablablabla")]
		public async Task Add_camera_with_invalid_rtsp_url(string rtspUrl)
		{
			await Assert.ThrowsAsync<HttpRequestException>(() => api.AddCamera("Front Yard", rtspUrl));
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Add_camera_with_duplicate_name()
		{
			await api.AddCamera(" Front Yard ", "rtsp://example.com/front");

			await Assert.ThrowsAsync<HttpRequestException>(() => api.AddCamera("Front Yard", "rtsp://example.com/backyard"));

			response.StatusCode.Should().Be(HttpStatusCode.Conflict);
		}
	}
}
