using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Rtsp.Cameras
{
	public static class Sql
	{
		const string SelectCameraSql = @"
			select
				CameraId as Id,
				Name,
				Nicknames as NicknamesJson,
				RtspUrl,
				CreatedAt
			from
				Camera";

		public static Task InsertCamera(this IDbConnection db, NewCamera camera)
		{
			return db.ExecuteAsync(@"insert into Camera (CameraId, Name, NickNames, RtspUrl)
				values (@id, @name, @nicknamesJson, @rtspUrl)",
				camera
			);
		}

		public static Task<IEnumerable<Camera>> SelectCameras(this IDbConnection db)
		{
			return db.QueryAsync<Camera>(SelectCameraSql);
		}

		public static Task<Camera> SelectCamera(this IDbConnection db, string id)
		{
			return db.QuerySingleOrDefaultAsync<Camera>($"{SelectCameraSql} where CameraId = @id", new { id });
		}

		public static Task<int> DeleteCamera(this IDbConnection db, string id)
		{
			return db.ExecuteAsync("delete from Camera where CameraId = @id", new { id });
		}
	}
}
