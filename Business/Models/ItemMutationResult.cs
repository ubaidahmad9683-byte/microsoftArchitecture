namespace MyProject.Business.Models;

public sealed record ItemMutationResult(ItemResponse? Item, bool WasDeleted = false);
