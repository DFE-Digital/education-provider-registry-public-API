using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.Testing.Logger;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentByUrn;
using DfE.EducationProviderRegistry.PublicApi.Api.Features.Establishments;
using DfE.EducationProviderRegistry.PublicApi.Api.UnitTests.Features.Establishments.TestDoubles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DfE.EducationProviderRegistry.PublicApi.Api.UnitTests.Features.Establishments;

public sealed class GetEstablishmentByUrnControllerTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        Mock<IUseCase<
            GetEstablishmentByUrnRequest,
            UseCaseResponse<EstablishmentDetailsReadModel>>> getEstablishmentUseCaseMock = new();

        Func<GetEstablishmentByUrnController> construct = () =>
            new(
                null!,
                getEstablishmentUseCaseMock.Object);

        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenGetEstablishmentUseCaseIsNull()
    {
        // Arrange
        ILogger<GetEstablishmentByUrnController> logger =
            new Mock<ILogger<GetEstablishmentByUrnController>>().Object;

        Func<GetEstablishmentByUrnController> construct = () =>
            new(
                logger,
                null!);

        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public async Task GetByUrnAsync_ReturnsBadRequest_WhenUseCaseResponseIsFailure()
    {
        // Arrange
        const string urn = "123456";

        InMemoryLogger<GetEstablishmentByUrnController> logger =
            ILoggerTestDouble.Default<GetEstablishmentByUrnController>();

        Mock<IUseCase<
            GetEstablishmentByUrnRequest,
            UseCaseResponse<EstablishmentDetailsReadModel>>> getEstablishmentUseCaseMock = new();

        UseCaseResponse<EstablishmentDetailsReadModel> failureReadModel =
            UseCaseResponse<EstablishmentDetailsReadModel>.Failure(
                EstablishmentDetailsReadModelTestDoubles.StubWithNoEstablishment(), "error");

        getEstablishmentUseCaseMock
            .Setup(useCase => useCase.HandleRequestAsync(
                It.IsAny<GetEstablishmentByUrnRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureReadModel);

        GetEstablishmentByUrnController sut = new(
            logger,
            getEstablishmentUseCaseMock.Object);

        // Act
        ActionResult<Establishment> result =
            await sut.GetByUrnAsync(urn, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);

        Assert.Contains(
            $"Getting establishment with URN {urn}.",
            logger.Logs);

        Assert.Contains(
            $"Failed to get establishment with URN {urn}.",
            logger.Logs);
    }

    [Fact]
    public async Task GetByUrnAsync_ReturnsNotFound_WhenEstablishmentDoesNotExist()
    {
        // Arrange
        const string urn = "123456";

        Mock<IUseCase<
            GetEstablishmentByUrnRequest,
            UseCaseResponse<EstablishmentDetailsReadModel>>> getEstablishmentUseCaseMock = new();

        UseCaseResponse<EstablishmentDetailsReadModel> successReadResponse =
            UseCaseResponse<EstablishmentDetailsReadModel>.Success(
                model: EstablishmentDetailsReadModelTestDoubles.StubWithNoEstablishment());

        getEstablishmentUseCaseMock
            .Setup(useCase => useCase.HandleRequestAsync(
                It.IsAny<GetEstablishmentByUrnRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(successReadResponse);

        InMemoryLogger<GetEstablishmentByUrnController> logger =
            ILoggerTestDouble.Default<GetEstablishmentByUrnController>();

        GetEstablishmentByUrnController sut = new(
            logger,
            getEstablishmentUseCaseMock.Object);

        // Act
        ActionResult<Establishment> result =
            await sut.GetByUrnAsync(urn, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);

        Assert.Contains(
            $"Getting establishment with URN {urn}.",
            logger.Logs);

        Assert.Contains(
            $"Establishment with URN {urn} was not found.",
            logger.Logs);
    }


    [Fact]
    public async Task GetByUrnAsync_PassesRequestAndCancellationTokenToUseCase()
    {
        // Arrange
        string urn = "123456";
        CancellationToken cancellationToken = new(true);

        GetEstablishmentByUrnRequest? capturedRequest = null;
        CancellationToken capturedCancellationToken = default;

        EstablishmentDetailsReadModel readModel =
            EstablishmentDetailsReadModelTestDoubles.StubWithNoEstablishment();

        Mock<IUseCase<
            GetEstablishmentByUrnRequest,
            UseCaseResponse<EstablishmentDetailsReadModel>>> getEstablishmentUseCaseMock = new();

        getEstablishmentUseCaseMock
            .Setup(useCase => useCase.HandleRequestAsync(
                It.IsAny<GetEstablishmentByUrnRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback<GetEstablishmentByUrnRequest, CancellationToken>(
                (request, token) =>
                {
                    capturedRequest = request;
                    capturedCancellationToken = token;
                })
            .ReturnsAsync(
                UseCaseResponse<EstablishmentDetailsReadModel>.Success(readModel));

        GetEstablishmentByUrnController sut = new(
            ILoggerTestDouble.Default<GetEstablishmentByUrnController>(),
            getEstablishmentUseCaseMock.Object);

        // Act
        await sut.GetByUrnAsync(urn, cancellationToken);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(urn, capturedRequest.Urn);
        Assert.Equal(cancellationToken, capturedCancellationToken);
    }

    [Fact]
    public async Task GetByUrnAsync_ReturnsOkWithEstablishment_WhenEstablishmentExists()
    {
        // Arrange
        EstablishmentDetailsReadModel readModel =
            EstablishmentDetailsReadModelTestDoubles.Stub();

        string expectedUrn =
            readModel.Establishment!.Identifiers.Urn;

        string expectedName =
            readModel.Establishment.Name.Value;

        Mock<IUseCase<
            GetEstablishmentByUrnRequest,
            UseCaseResponse<EstablishmentDetailsReadModel>>> getEstablishmentUseCaseMock = new();

        UseCaseResponse<EstablishmentDetailsReadModel> successfulReadModel =
            UseCaseResponse<EstablishmentDetailsReadModel>.Success(readModel);

        getEstablishmentUseCaseMock
            .Setup(useCase => useCase.HandleRequestAsync(
                It.IsAny<GetEstablishmentByUrnRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(successfulReadModel);

        InMemoryLogger<GetEstablishmentByUrnController> logger =
            ILoggerTestDouble.Default<GetEstablishmentByUrnController>();

        GetEstablishmentByUrnController sut = new(
            logger,
            getEstablishmentUseCaseMock.Object);

        // Act
        ActionResult<Establishment> result =
            await sut.GetByUrnAsync(
                expectedUrn,
                TestContext.Current.CancellationToken);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        Establishment establishment =
            Assert.IsType<Establishment>(okResult.Value);

        Assert.Equal(expectedUrn, establishment.Urn);
        Assert.Equal(expectedName, establishment.Name);

        Assert.Contains(
            $"Getting establishment with URN {expectedUrn}.",
            logger.Logs);
    }
}
