using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace FarmManager.IntegrationTest.Fixtures
{
    [CollectionDefinition(nameof(ApiFixtureCollection))]
    public class ApiFixtureCollection :
        ICollectionFixture<WebApplicationFixture<Program>>,
        ICollectionFixture<DatabaseFixture>,
        ICollectionFixture<MessagingFixture>
    {
    }
}
