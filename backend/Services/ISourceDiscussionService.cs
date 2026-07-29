using ArgosHound.Api.Contracts;
using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public interface ISourceDiscussionService
{
    IReadOnlyList<SourceDiscussion> GetAll();

    SourceDiscussion Get(Guid id);

    SourceDiscussion Create(CreateSourceDiscussionRequest request);
}
