using ArgosHound.Api.Contracts;
using ArgosHound.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArgosHound.Api.Controllers;

[ApiController]
[Route("api/opportunities")]
public sealed class OpportunitiesController(
    IOpportunityRepository opportunityRepository,
    IOpportunityReportService reportService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OpportunitySummaryResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var opportunities = await opportunityRepository.GetAllAsync(
            cancellationToken);
        return Ok(opportunities.Select(item => new OpportunitySummaryResponse(
            item.Id,
            item.DiscussionId,
            item.Type,
            item.Problem,
            item.ProblemInferred,
            item.Topic,
            item.Score,
            item.Confidence,
            item.SuggestedAction,
            item.CreatedAt)));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OpportunityDetailResponse>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var opportunity = await opportunityRepository.GetAsync(
            id,
            cancellationToken);
        return opportunity is null
            ? NotFound()
            : Ok(reportService.Build(opportunity));
    }
}
