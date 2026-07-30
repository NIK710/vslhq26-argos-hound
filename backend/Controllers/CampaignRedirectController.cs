using ArgosHound.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArgosHound.Api.Controllers;

[ApiController]
[Route("r")]
public sealed class CampaignRedirectController(
    ICampaignLinkService campaignLinkService) : ControllerBase
{
    [HttpGet("{code}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Open(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await campaignLinkService.OpenAsync(
            code,
            cancellationToken);

        return result.Status switch
        {
            CampaignRedirectStatus.Found => RedirectWithoutReferrer(
                result.DestinationUrl!),
            CampaignRedirectStatus.Expired => StatusCode(
                StatusCodes.Status410Gone),
            _ => NotFound(),
        };
    }

    private IActionResult RedirectWithoutReferrer(string destinationUrl)
    {
        Response.Headers.Append("Referrer-Policy", "no-referrer");
        Response.Headers.Append("Cache-Control", "no-store");
        return Redirect(destinationUrl);
    }
}
