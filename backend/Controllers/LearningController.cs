using ArgosHound.Api.Contracts;
using ArgosHound.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArgosHound.Api.Controllers;

[ApiController]
[Route("api/learning")]
public sealed class LearningController(LearningService service) : ControllerBase
{
    [HttpGet("summary")]
    public Task<LearningSummaryResponse> GetSummary(CancellationToken token) =>
        service.GetSummaryAsync(token);
}
