using System.Net;
using System.Net.Http.Json;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Establishments;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.PublicApi.Api.Features.Establishments;

namespace DfE.EducationProviderRegistry.PublicApi.Api.SystemTests;

public sealed class GetEstablishmentsTests : WebApplicationFactoryBaseSystemTest
{
    public GetEstablishmentsTests(IServiceProvider provider) : base(provider)
    {
    }

    [Fact]
    public async Task Get_Establishments_Returns_Establishments()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyCollection<EstablishmentAggregate> seededEstablishments =
            EstablishmentAggregateTestDoubles.StubMany(count: 10);

        await DatabaseFixture.SeedAsync<IEnumerable<EstablishmentAggregate>, EstablishmentAggregates>(seededEstablishments, ct);

        using HttpClient client = Factory.CreateClient();

        // Act
        using HttpResponseMessage httpResponse =
            await client.GetAsync("/api/v1/establishments", TestContext.Current.CancellationToken);

        // Assert
        IEnumerable<Establishment>? response =
            await httpResponse.Content.ReadFromJsonAsync<IEnumerable<Establishment>>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.NotNull(response);
        Establishment[] mappedSeededEstablishments = MapEstablishments(seededEstablishments);
        Assert.Equivalent(mappedSeededEstablishments, response);
    }

    private static Establishment[] MapEstablishments(IEnumerable<EstablishmentAggregate> inputs)
    {
        return [.. inputs.Select(MapEstablishment)];
    }

    private static Establishment MapEstablishment(EstablishmentAggregate input)
    {
        return new()
        {
            Name = input.Name,
            Urn = input.Urn
        };
    }
}
