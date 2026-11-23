using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;

namespace DfE.EducationProviderRegistry.PublicApi.Api.UnitTests.Features.Establishments.TestDoubles;

internal static class EstablishmentDetailsReadModelTestDoubles
{
    public static EstablishmentDetailsReadModel Stub()
    {
        EstablishmentDetailsBuilder builder = EstablishmentDetailsBuilder.Create();

        EstablishmentDetailsModel model = builder.Build();

        return new EstablishmentDetailsReadModel()
        {
            Establishment = model
        };
    }

    internal static EstablishmentDetailsReadModel StubWithNoEstablishment()
    {
        return new EstablishmentDetailsReadModel()
        {
            Establishment = null
        };
    }
}
