namespace MyProject.Business.Models;

public sealed class ItemRequest
{
    [Range(0, int.MaxValue)]
    public int Id { get; init; }

    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; init; }

    [Range(typeof(decimal), "0", "9999999999999999.99")]
    public decimal Price { get; init; }

    [EnumDataType(typeof(ItemState))]
    public ItemState ItemState { get; init; }
}
