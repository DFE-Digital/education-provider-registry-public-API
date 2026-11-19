using Asp.Versioning;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments;
using DfE.EducationProviderRegistry.IntegrationsApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.IntegrationsApi.Api.Features.Establishments;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/establishments")]
[ApiController]
public sealed class GetEstablishmentsController : ControllerBase
{
    private readonly ILogger<GetEstablishmentsController> _logger;
    private readonly IUseCase<
        GetEstablishmentsRequest,
        UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>> _getEstablishmentsUseCase;

    public GetEstablishmentsController(
        ILogger<GetEstablishmentsController> logger,
        IUseCase<
            GetEstablishmentsRequest,
            UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>> getEstablishmentsUseCase)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(getEstablishmentsUseCase);

        _logger = logger;
        _getEstablishmentsUseCase = getEstablishmentsUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<Establishment>>> GetEstablishmentsAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting establishments.");

        GetEstablishmentsRequest request = new();

        UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>> response =
            await _getEstablishmentsUseCase.HandleRequestAsync(
                request,
                cancellationToken);

        if (!response.SuccessfulRequest)
        {
            _logger.LogWarning("Failed to get establishments.");

            return BadRequest();
        }

        Establishment[] establishments = response.Model
            .Select(establishment => new Establishment
            {
                Urn = establishment.Identifiers.Urn,
                Name = establishment.Name.Value,
            })
            .ToArray();


        _logger.LogInformation(
            "Retrieved {EstablishmentCount} establishments.",
            establishments.Length);

        return Ok(establishments);
    }
}
