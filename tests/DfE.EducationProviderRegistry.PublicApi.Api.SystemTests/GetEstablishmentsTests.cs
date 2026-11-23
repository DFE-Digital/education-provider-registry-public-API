using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DfE.EducationProviderRegistry.PublicApi.Api.SystemTests;

public sealed class GetEstablishmentsTests
{
    [Fact]
    public async Task Get_Establishments_Returns_Establishments()
    {
        // Arrange

        // Act
        WebApplicationFactory<Program> factory = new();
        // Assert
    }
}
