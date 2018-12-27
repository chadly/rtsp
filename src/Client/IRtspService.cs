using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rtsp
{
	public interface IRtspService
	{
		Task<IEnumerable<Camera>> GetCameras();
		Task<Camera> GetCamera(string id);

		Task<Camera> AddCamera(Camera camera);
		Task RemoveCamera(string id);
	}
}
