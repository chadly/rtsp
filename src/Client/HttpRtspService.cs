using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Archon.Http;
using Newtonsoft.Json;

namespace Rtsp
{
	public class HttpRtspService : IRtspService
	{
		readonly HttpClient api;

		public HttpRtspService(HttpClient api)
		{
			this.api = api;
		}

		public async Task<IEnumerable<Camera>> GetCameras()
		{
			var response = await api.GetAsync("/cameras/");
			await response.EnsureSuccess();
			return await response.Content.ReadJsonAsync<IEnumerable<Camera>>();
		}

		public async Task<Camera> GetCamera(string id)
		{
			var response = await api.GetAsync($"/cameras/{id}");

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			await response.EnsureSuccess();
			return await response.Content.ReadJsonAsync<Camera>();
		}

		public async Task<Camera> AddCamera(Camera camera)
		{
			string json = JsonConvert.SerializeObject(camera);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await api.PostAsync("/cameras/", content);
			await response.EnsureSuccess();
			return await response.Content.ReadJsonAsync<Camera>();
		}

		public async Task RemoveCamera(string id)
		{
			var response = await api.DeleteAsync($"/cameras/{id}");

			if (response.StatusCode == HttpStatusCode.NotFound)
				return;

			await response.EnsureSuccess();
		}
	}
}
