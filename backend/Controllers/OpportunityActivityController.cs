using ArgosHound.Api.Contracts;
using ArgosHound.Api.Models;
using ArgosHound.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArgosHound.Api.Controllers;

[ApiController]
[Route("api/opportunities/{opportunityId:guid}")]
public sealed class OpportunityActivityController(
    OpportunityActivityService service) : ControllerBase
{
    [HttpGet("activity")]
    public async Task<ActionResult<OpportunityActivityResponse>> Get(
        Guid opportunityId, CancellationToken token)
    {
        var result = await service.GetAsync(opportunityId, token);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("decisions")]
    public async Task<ActionResult<BuilderDecision>> Decide(
        Guid opportunityId, RecordDecisionRequest request, CancellationToken token)
    {
        var result = await service.DecideAsync(opportunityId, request, token);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("outcomes")]
    public async Task<ActionResult<Outcome>> Outcome(
        Guid opportunityId, RecordOutcomeRequest request, CancellationToken token)
    {
        var result = await service.AddOutcomeAsync(opportunityId, request, token);
        return result is null ? NotFound() : Ok(result);
    }
}
