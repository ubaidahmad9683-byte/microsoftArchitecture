namespace MyProject.Repository;

public sealed class ItemRepository(ApplicationDbContext dbContext) : IItemRepository
{
    public async Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Items
            .AsNoTracking()
            .OrderBy(item => item.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Item?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.Items
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<Item> AddAsync(Item item, CancellationToken cancellationToken)
    {
        dbContext.Items.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task UpdateAsync(Item item, CancellationToken cancellationToken)
    {
        dbContext.Items.Update(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Item item, CancellationToken cancellationToken)
    {
        dbContext.Items.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
