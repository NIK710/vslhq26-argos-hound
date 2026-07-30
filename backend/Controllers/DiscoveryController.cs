using ArgosHound.Api.Contracts;
using ArgosHound.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArgosHound.Api.Controllers;

[ApiController]
[Route("api/discovery")]
public sealed class DiscoveryController(
    IDiscoveryService discoveryService,
    IOpportunityReportService reportService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OpportunityDetailResponse>> Discover(
        CreateDiscoveryRequest request,
        CancellationToken cancellationToken)
    {
        if (request.DiscussionId == Guid.Empty)
        {
            return ValidationProblem(
                detail: "discussionId must be a non-empty UUID.");
        }

        try
        {
            var opportunity = await discoveryService.DiscoverAsync(
                request.DiscussionId,
                cancellationToken);
            var report = await reportService.BuildAsync(
                opportunity,
                cancellationToken);

            return Created($"/api/opportunities/{opportunity.Id}", report);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (LlmAnalysisTimeoutException)
        {
            return Problem(
                title: "Discovery timed out",
                detail: "The agent took too long to respond. Please try again.",
                statusCode: StatusCodes.Status504GatewayTimeout);
        }
        catch (LlmAnalysisOutputException)
        {
            return Problem(
                title: "Discovery result could not be validated",
                detail:
                    "The agent returned an unsafe or malformed result. Nothing was saved; please try again.",
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (LlmAnalysisUnavailableException)
        {
            return Problem(
                title: "Discovery service unavailable",
                detail:
                    "The Foundry analysis service is temporarily unavailable. Please try again.",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }
}
