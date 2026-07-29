using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public interface IProfileImportService
{
    ProfileImport Create(AssistantProvider provider, string content);

    ProfileImport Get(Guid id);

    ProfileImport Update(Guid id, ProposedBuilderProfile proposedProfile);

    BuilderProfile Approve(Guid id);

    ProfileImport Reject(Guid id);

    void Delete(Guid id);
}
