using ArgosHound.Api.Contracts;
using ArgosHound.Api.Models;
using ArgosHound.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArgosHound.Api.Controllers;

[ApiController]
[Route("api/sources/discussions")]
public sealed class SourcesController(
    ISourceDiscussionService sourceDiscussionService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<SourceDiscussion>> GetDiscussions() =>
        Ok(sourceDiscussionService.GetAll());

    [HttpGet("{id:guid}")]
    public ActionResult<SourceDiscussion> GetDiscussion(Guid id)
    {
        try
        {
            return Ok(sourceDiscussionService.Get(id));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public ActionResult<SourceDiscussion> CreateDiscussion(
        CreateSourceDiscussionRequest request)
    {
        try
        {
            var discussion = sourceDiscussionService.Create(request);
            return CreatedAtAction(
                nameof(GetDiscussion),
                new { id = discussion.Id },
                discussion);
        }
        catch (ArgumentException exception)
        {
            return ValidationProblem(detail: exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Detail = exception.Message });
        }
    }
}
