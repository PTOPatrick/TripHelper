namespace TripHelper.Api.IntegrationTests.Common;

[CollectionDefinition(CollectionName)]
public class TripHelperApiFactoryCollection : ICollectionFixture<TripHelperApiFactory>
{
    public const string CollectionName = "TripHelperApiFactoryCollection";
}