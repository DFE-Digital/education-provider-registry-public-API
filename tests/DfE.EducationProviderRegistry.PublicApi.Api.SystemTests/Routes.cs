namespace DfE.EducationProviderRegistry.PublicApi.Api.SystemTests;

public static class Routes
{
    private const string EstablishmentsV1Base = "/api/v1/establishments";
    public static Uri GetEstablishments => new(EstablishmentsV1Base);
    public static Uri GetEstablishmentsByUrn(string urn) => new(EstablishmentsV1Base + $"/{urn}");
}
