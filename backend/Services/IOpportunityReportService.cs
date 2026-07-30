using ArgosHound.Api.Contracts;
using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public interface IOpportunityReportService
{
    OpportunityDetailResponse Build(Opportunity opportunity);
}
