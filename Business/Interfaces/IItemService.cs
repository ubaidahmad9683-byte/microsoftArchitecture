

namespace MyProject.Business.Interfaces;

public interface IItemService
{
    Task<IReadOnlyList<ItemResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<ItemResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<ItemMutationResult?> ProcessAsync(
        ItemRequest request,
        CancellationToken cancellationToken);
}
