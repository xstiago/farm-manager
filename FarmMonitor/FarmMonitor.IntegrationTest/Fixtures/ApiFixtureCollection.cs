using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace FarmMonitor.IntegrationTest.Fixtures
{
    [CollectionDefinition(nameof(ApiFixtureCollection))]
    public class ApiFixtureCollection :
        ICollectionFixture<WebApplicationFixture<Program>>,
        ICollectionFixture<DatabaseFixture>,
        ICollectionFixture<MessagingFixture>
    {
    }
}
