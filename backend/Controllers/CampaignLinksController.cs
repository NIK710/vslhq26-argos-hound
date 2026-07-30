using ArgosHound.Api.Contracts;
using ArgosHound.Api.Configuration;
using ArgosHound.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ArgosHound.Api.Controllers;

[ApiController]
[Route("api/opportunities/{opportunityId:guid}/campaign-links")]
public sealed class CampaignLinksController(
    ICampaignLinkService campaignLinkService,
    IOptions<CampaignOptions> options) : ControllerBase
{
    private readonly Uri _publicBaseUrl =
        new(options.Value.PublicBaseUrl.TrimEnd('/') + "/");

    [HttpPost]
    public async Task<ActionResult<CreateCampaignLinkResponse>> Create(
        Guid opportunityId,
        CreateCampaignLinkRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await campaignLinkService.CreateAsync(
                opportunityId,
                request.DestinationUrl,
                request.Purpose,
                request.ExpiresAt,
                cancellationToken);
            var redirectUrl = new Uri(
                _publicBaseUrl,
                $"r/{created.Code}").AbsoluteUri;

            return Created(
                redirectUrl,
                new CreateCampaignLinkResponse(
                    created.Campaign,
                    redirectUrl));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException exception)
        {
            return ValidationProblem(detail: exception.Message);
        }
    }
}
