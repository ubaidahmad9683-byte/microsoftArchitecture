namespace MyProject.Business;

public sealed class ItemService(IItemRepository itemRepository) : IItemService
{
    public async Task<IReadOnlyList<ItemResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var items = await itemRepository.GetAllAsync(cancellationToken);
        return items.Select(MapToResponse).ToArray();
    }

    public async Task<ItemResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(id, cancellationToken);
        return item is null ? null : MapToResponse(item);
    }

    public async Task<ItemMutationResult?> ProcessAsync(
        ItemRequest request,
        CancellationToken cancellationToken)
    {
        return request.ItemState switch
        {
            ItemState.Add => await AddAsync(request, cancellationToken),
            ItemState.AddOrEdit when request.Id == 0 =>
                await AddAsync(request, cancellationToken),
            ItemState.AddOrEdit => await UpdateAsync(request, cancellationToken),
            ItemState.Delete => await DeleteAsync(request.Id, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(
                nameof(request),
                request.ItemState,
                "The item state is not supported.")
        };
    }

    private async Task<ItemMutationResult> AddAsync(
        ItemRequest request,
        CancellationToken cancellationToken)
    {
        var item = new Item
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ItemState = request.ItemState
        };

        var createdItem = await itemRepository.AddAsync(item, cancellationToken);
        return new ItemMutationResult(MapToResponse(createdItem));
    }

    private async Task<ItemMutationResult?> UpdateAsync(
        ItemRequest request,
        CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
        {
            return null;
        }

        item.Name = request.Name;
        item.Description = request.Description;
        item.Price = request.Price;
        item.ItemState = request.ItemState;

        await itemRepository.UpdateAsync(item, cancellationToken);
        return new ItemMutationResult(MapToResponse(item));
    }

    private async Task<ItemMutationResult?> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return null;
        }

        await itemRepository.DeleteAsync(item, cancellationToken);
        return new ItemMutationResult(Item: null, WasDeleted: true);
    }

    private static ItemResponse MapToResponse(Item item)
    {
        return new ItemResponse(
            item.Id,
            item.Name,
            item.Description,
            item.Price,
            item.ItemState);
    }
}
