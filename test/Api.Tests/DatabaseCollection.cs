using Xunit;

namespace Rtsp.Tests
{
	[CollectionDefinition(Name)]
	public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
	{
		public const string Name = "Database";
	}
}
