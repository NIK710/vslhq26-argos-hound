using ArgosHound.Api.Contracts;
using ArgosHound.Api.Models;
using ArgosHound.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArgosHound.Api.Controllers;

[ApiController]
[Route("api/analysis/discussions")]
public sealed class AnalysisController(
    IBuilderProfileStore builderProfileStore,
    IProductCatalog productCatalog,
    ISourceDiscussionService sourceDiscussionService,
    ILlmAnalysisProvider analysisProvider) : ControllerBase
{
    [HttpPost("{discussionId:guid}")]
    public async Task<ActionResult<AnalyzeDiscussionResponse>> Analyze(
        Guid discussionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var builder = builderProfileStore.Get();
            var context = new OpportunityAnalysisContext(
                builder,
                productCatalog.GetForBuilder(builder.Id),
                sourceDiscussionService.Get(discussionId));
            var analysis = await analysisProvider.AnalyzeAsync(
                context,
                cancellationToken);

            return Ok(new AnalyzeDiscussionResponse(
                discussionId,
                analysis,
                DateTimeOffset.UtcNow));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (LlmAnalysisTimeoutException)
        {
            return Problem(
                title: "Analysis timed out",
                detail: "The agent took too long to respond. Please try again.",
                statusCode: StatusCodes.Status504GatewayTimeout);
        }
        catch (LlmAnalysisOutputException)
        {
            return Problem(
                title: "Analysis could not be validated",
                detail:
                    "The agent returned an unsafe or malformed result. No analysis was accepted; please try again.",
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (LlmAnalysisUnavailableException)
        {
            return Problem(
                title: "Analysis service unavailable",
                detail:
                    "The Foundry analysis service is temporarily unavailable. Please try again.",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }
}
