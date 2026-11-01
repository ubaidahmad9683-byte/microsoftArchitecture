namespace MyProject.Business.Models;

public sealed record ItemResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    ItemState ItemState);
