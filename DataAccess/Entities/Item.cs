namespace MyProject.DataAccess.Entities;

public sealed class Item
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public ItemState ItemState { get; set; }
}
