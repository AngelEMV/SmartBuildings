using Xunit;

namespace GuestActionsProcessor.IntegrationTests.Infrastructure
{
	[CollectionDefinition(Name)]
	public class TestsCollection : ICollectionFixture<TestHost>
	{
		public const string Name = nameof(TestsCollection);
	}
}
