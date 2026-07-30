using ArgosHound.Api.Services;
using Azure;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArgosHound.Api.Controllers;

[ApiController]
[Route("api/health/foundry-agent")]
public sealed class FoundryAgentHealthController(
    IFoundryAgentConnectivityService connectivityService,
    IHostEnvironment hostEnvironment) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<FoundryAgentConnectivityResult>> Get(
        CancellationToken cancellationToken)
    {
        if (!hostEnvironment.IsDevelopment())
        {
            return NotFound();
        }

        try
        {
            return Ok(await connectivityService.CheckAsync(cancellationToken));
        }
        catch (Exception exception) when (
            exception is AuthenticationFailedException
            or RequestFailedException
            or InvalidOperationException)
        {
            return Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Foundry agent connectivity check failed");
        }
    }
}
