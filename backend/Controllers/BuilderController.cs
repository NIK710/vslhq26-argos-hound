using ArgosHound.Api.Contracts;
using ArgosHound.Api.Models;
using ArgosHound.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArgosHound.Api.Controllers;

[ApiController]
[Route("api/builder")]
public sealed class BuilderController(
    IBuilderProfileStore builderProfileStore,
    IProfileImportService profileImportService,
    IHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    public ActionResult<BuilderProfile> GetBuilder() =>
        Ok(builderProfileStore.Get());

    [HttpGet("profile-export-prompt")]
    public async Task<IActionResult> GetProfileExportPrompt(
        CancellationToken cancellationToken)
    {
        var path = Path.Combine(
            environment.ContentRootPath,
            "Prompts",
            "profile-import.md");
        return Content(
            await System.IO.File.ReadAllTextAsync(path, cancellationToken),
            "text/plain");
    }

    [HttpPost("profile-imports")]
    public ActionResult<ProfileImport> CreateImport(CreateProfileImportRequest request)
    {
        try
        {
            var import = profileImportService.Create(request.Provider, request.Content);
            return CreatedAtAction(nameof(GetImport), new { id = import.Id }, import);
        }
        catch (ArgumentException exception)
        {
            return ValidationProblem(detail: exception.Message);
        }
    }

    [HttpGet("profile-imports/{id:guid}")]
    public ActionResult<ProfileImport> GetImport(Guid id)
    {
        try
        {
            return Ok(profileImportService.Get(id));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("profile-imports/{id:guid}")]
    public ActionResult<ProfileImport> UpdateImport(
        Guid id,
        UpdateProfileImportRequest request)
    {
        try
        {
            return Ok(profileImportService.Update(id, request.ProposedProfile));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
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

    [HttpPost("profile-imports/{id:guid}/approve")]
    public ActionResult<BuilderProfile> ApproveImport(Guid id)
    {
        try
        {
            return Ok(profileImportService.Approve(id));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Detail = exception.Message });
        }
    }

    [HttpPost("profile-imports/{id:guid}/reject")]
    public ActionResult<ProfileImport> RejectImport(Guid id)
    {
        try
        {
            return Ok(profileImportService.Reject(id));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Detail = exception.Message });
        }
    }

    [HttpDelete("profile-imports/{id:guid}")]
    public IActionResult DeleteImport(Guid id)
    {
        try
        {
            profileImportService.Delete(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
