using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.IntegrationsApi.Api.Features.Health;


[Route("health")]
[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet(Name = "HealthCheck")]
    public IActionResult Index()
    {
        return Ok(new
        {
            status = "Service is running",
            timestamp = DateTime.UtcNow
        });
    }
}
