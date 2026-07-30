using ArgosHound.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArgosHound.Api.Controllers;

[ApiController]
[Route("api/health/azure-openai")]
public sealed class AzureOpenAIHealthController(
    IAzureOpenAIConnectivityService connectivityService,
    IHostEnvironment hostEnvironment) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AzureOpenAIConnectivityResult>> Get(
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
        catch (HttpRequestException exception)
        {
            return Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Azure OpenAI connectivity check failed");
        }
    }
}
