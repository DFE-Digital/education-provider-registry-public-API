using Asp.Versioning;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentByUrn;
using DfE.EducationProviderRegistry.IntegrationsApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.IntegrationsApi.Api.Features.Establishments;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/establishments")]
[ApiController]
public sealed class GetEstablishmentByUrnController : ControllerBase
{
    private readonly ILogger<GetEstablishmentByUrnController> _logger;
    private readonly IUseCase<
        GetEstablishmentByUrnRequest,
        UseCaseResponse<EstablishmentDetailsReadModel>> _getEstablishmentUseCase;

    public GetEstablishmentByUrnController(
        ILogger<GetEstablishmentByUrnController> logger,
        IUseCase<
            GetEstablishmentByUrnRequest,
            UseCaseResponse<EstablishmentDetailsReadModel>> getEstablishmentUseCase)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(getEstablishmentUseCase);

        _logger = logger;
        _getEstablishmentUseCase = getEstablishmentUseCase;
    }

    [HttpGet("{urn}")]
    public async Task<ActionResult<Establishment>> GetByUrnAsync(
        string urn,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Getting establishment with URN {Urn}.",
            urn);

        GetEstablishmentByUrnRequest request = new(urn);

        UseCaseResponse<EstablishmentDetailsReadModel> response =
            await _getEstablishmentUseCase.HandleRequestAsync(
                request,
                cancellationToken);

        if (!response.SuccessfulRequest)
        {
            _logger.LogWarning(
                "Failed to get establishment with URN {Urn}.",
                urn);

            return BadRequest();
        }

        if (response.Model.Establishment is null)
        {
            _logger.LogInformation(
                "Establishment with URN {Urn} was not found.",
                urn);

            return NotFound();
        }

        Establishment establishment = new()
        {
            Urn = response.Model.Establishment.Identifiers.Urn,
            Name = response.Model.Establishment.Name.Value
        };

        return Ok(establishment);
    }
}


