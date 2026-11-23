using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.Testing.Logger;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments;
using DfE.EducationProviderRegistry.PublicApi.Api.Features.Establishments;
using DfE.EducationProviderRegistry.PublicApi.Api.UnitTests.Features.Establishments.TestDoubles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DfE.EducationProviderRegistry.PublicApi.Api.UnitTests.Features.Establishments;

public sealed class GetEstablishmentsControllerTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        Mock<IUseCase<
            GetEstablishmentsRequest,
            UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>>>
            getEstablishmentsUseCaseMock = new();

        Func<GetEstablishmentsController> construct = () =>
            new(
                null!,
                getEstablishmentsUseCaseMock.Object);

        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenGetEstablishmentsUseCaseIsNull()
    {
        // Arrange
        ILogger<GetEstablishmentsController> logger =
            new Mock<ILogger<GetEstablishmentsController>>().Object;

        Func<GetEstablishmentsController> construct = () =>
            new(
                logger,
                null!);

        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public async Task GetEstablishmentsAsync_ReturnsBadRequest_WhenUseCaseResponseIsFailure()
    {
        // Arrange
        InMemoryLogger<GetEstablishmentsController> logger =
            ILoggerTestDouble.Default<GetEstablishmentsController>();

        Mock<IUseCase<
            GetEstablishmentsRequest,
            UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>>>
            getEstablishmentsUseCaseMock = new();

        UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>> failureResponse =
            UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>.Failure(
                model: EstablishmentDetailsCollectionTestDoubles.Empty(),
                error: "error");

        getEstablishmentsUseCaseMock
            .Setup(useCase => useCase.HandleRequestAsync(
                It.IsAny<GetEstablishmentsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResponse);

        GetEstablishmentsController sut = new(
            logger,
            getEstablishmentsUseCaseMock.Object);

        // Act
        ActionResult<IReadOnlyCollection<Establishment>> result =
            await sut.GetEstablishmentsAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);

        Assert.Contains(
            "Getting establishments.",
            logger.Logs);

        Assert.Contains(
            "Failed to get establishments.",
            logger.Logs);
    }

    [Fact]
    public async Task GetEstablishmentsAsync_PassesRequestAndCancellationTokenToUseCase()
    {
        // Arrange
        CancellationToken cancellationToken = new(true);

        GetEstablishmentsRequest? capturedRequest = null;
        CancellationToken capturedCancellationToken = default;

        Mock<IUseCase<
            GetEstablishmentsRequest,
            UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>>>
            getEstablishmentsUseCaseMock = new();

        UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>> successfulResponse =
            UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>.Success(
                model: EstablishmentDetailsCollectionTestDoubles.Empty());

        getEstablishmentsUseCaseMock
            .Setup(useCase => useCase.HandleRequestAsync(
                It.IsAny<GetEstablishmentsRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback<GetEstablishmentsRequest, CancellationToken>(
                (request, token) =>
                {
                    capturedRequest = request;
                    capturedCancellationToken = token;
                })
            .ReturnsAsync(successfulResponse);

        GetEstablishmentsController sut = new(
            ILoggerTestDouble.Default<GetEstablishmentsController>(),
            getEstablishmentsUseCaseMock.Object);

        // Act
        await sut.GetEstablishmentsAsync(cancellationToken);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(cancellationToken, capturedCancellationToken);
    }

    [Fact]
    public async Task GetEstablishmentsAsync_ReturnsOkWithEmptyCollection_WhenNoEstablishmentsExist()
    {
        // Arrange
        Mock<IUseCase<
            GetEstablishmentsRequest,
            UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>>> getEstablishmentsUseCaseMock = new();

        UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>> successfulResponse =
            UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>.Success(
                model: EstablishmentDetailsCollectionTestDoubles.Empty());

        getEstablishmentsUseCaseMock
            .Setup(useCase => useCase.HandleRequestAsync(
                It.IsAny<GetEstablishmentsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(successfulResponse);

        InMemoryLogger<GetEstablishmentsController> logger =
            ILoggerTestDouble.Default<GetEstablishmentsController>();

        GetEstablishmentsController sut = new(
            logger,
            getEstablishmentsUseCaseMock.Object);

        // Act
        ActionResult<IReadOnlyCollection<Establishment>> result =
            await sut.GetEstablishmentsAsync(
                TestContext.Current.CancellationToken);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        Establishment[] establishments =
            Assert.IsType<Establishment[]>(okResult.Value);

        Assert.Empty(establishments);

        Assert.Contains(
            "Getting establishments.",
            logger.Logs);

        Assert.Contains(
            "Retrieved 0 establishments.",
            logger.Logs);
    }

    [Fact]
    public async Task GetEstablishmentsAsync_ReturnsOkWithMappedEstablishments_WhenEstablishmentsExist()
    {
        // Arrange
        IReadOnlyCollection<EstablishmentDetailsModel> readModels =
            EstablishmentDetailsCollectionTestDoubles.Stub();

        Mock<IUseCase<
            GetEstablishmentsRequest,
            UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>>> getEstablishmentsUseCaseMock = new();

        UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>
            successfulResponse =
                UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>.Success(readModels);

        getEstablishmentsUseCaseMock
            .Setup(useCase => useCase.HandleRequestAsync(
                It.IsAny<GetEstablishmentsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(successfulResponse);

        InMemoryLogger<GetEstablishmentsController> logger =
            ILoggerTestDouble.Default<GetEstablishmentsController>();

        GetEstablishmentsController sut = new(
            logger,
            getEstablishmentsUseCaseMock.Object);

        // Act
        ActionResult<IReadOnlyCollection<Establishment>> result =
            await sut.GetEstablishmentsAsync(
                TestContext.Current.CancellationToken);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        Establishment[] establishments =
            Assert.IsType<Establishment[]>(okResult.Value);

        Assert.Equal(readModels.Count, establishments.Length);

        foreach (EstablishmentDetailsModel readModel in readModels)
        {
            Establishment establishment =
                Assert.Single(
                    establishments,
                    establishment =>
                        establishment.Urn == readModel.Identifiers.Urn);

            Assert.Equal(
                readModel.Name.Value,
                establishment.Name);
        }

        Assert.Contains(
            "Getting establishments.",
            logger.Logs);

        Assert.Contains(
            $"Retrieved {readModels.Count} establishments.",
            logger.Logs);
    }
}


