using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public interface IProductCatalog
{
    IReadOnlyList<Product> GetForBuilder(Guid builderId);
}
