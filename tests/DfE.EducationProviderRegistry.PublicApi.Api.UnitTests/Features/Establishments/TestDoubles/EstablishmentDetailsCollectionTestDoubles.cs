using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;

namespace DfE.EducationProviderRegistry.PublicApi.Api.UnitTests.Features.Establishments.TestDoubles;

internal static class EstablishmentDetailsCollectionTestDoubles
{
    public static IReadOnlyCollection<EstablishmentDetailsModel> Stub()
    {
        EstablishmentDetailsBuilder builder =
            EstablishmentDetailsBuilder.Create();

        EstablishmentDetailsModel model =
            builder.Build();

        return new[]
        {
            model
        };
    }

    public static IReadOnlyCollection<EstablishmentDetailsModel> Empty()
    {
        return Array.Empty<EstablishmentDetailsModel>();
    }
}
