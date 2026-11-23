namespace DfE.EducationProviderRegistry.PublicApi.Api.SystemTests;

public static class Routes
{
    private const string EstablishmentsV1Base = "/api/v1/establishments";
    public static Uri GetEstablishments => new(EstablishmentsV1Base, UriKind.Relative);
    public static Uri GetEstablishmentsByUrn(string urn) => new($"{EstablishmentsV1Base}/{urn}", UriKind.Relative);
}
