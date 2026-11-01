namespace MyProject.Repository.Interfaces;

public interface IItemRepository
{
    Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken);

    Task<Item?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Item> AddAsync(Item item, CancellationToken cancellationToken);

    Task UpdateAsync(Item item, CancellationToken cancellationToken);

    Task DeleteAsync(Item item, CancellationToken cancellationToken);
}
