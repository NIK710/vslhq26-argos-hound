using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public interface IBuilderProfileStore
{
    BuilderProfile Get();

    BuilderProfile ReplaceWith(ProposedBuilderProfile proposedProfile);
}
