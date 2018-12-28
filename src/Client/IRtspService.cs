using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rtsp
{
	public interface IRtspService
	{
		Task<IEnumerable<Camera>> GetCameras();
		Task<Camera> GetCamera(int id);

		Task<Camera> AddCamera(string name, string rtspUrl);
		Task RemoveCamera(int id);
	}
}
