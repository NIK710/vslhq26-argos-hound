using ArgosHound.Api.Data;
using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public sealed class DemoProductCatalog : IProductCatalog
{
    public IReadOnlyList<Product> GetForBuilder(Guid builderId) =>
        DemoData.Products
            .Where(product => product.BuilderId == builderId)
            .Select(Clone)
            .ToArray();

    private static Product Clone(Product source) =>
        new()
        {
            Id = source.Id,
            BuilderId = source.BuilderId,
            Name = source.Name,
            Description = source.Description,
            Capabilities = [.. source.Capabilities],
            TargetUsers = [.. source.TargetUsers],
            ProductUrl = source.ProductUrl,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
        };
}
