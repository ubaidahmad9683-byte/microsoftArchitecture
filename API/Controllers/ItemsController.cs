namespace MyProject.API.Controllers;

[ApiController]
[Route("api/items")]
public sealed class ItemsController(IItemService itemService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ItemResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await itemService.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest("Id must be greater than zero.");
        }

        var item = await itemService.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemResponse>> Process(
        [FromBody] ItemRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ItemState == ItemState.Delete && request.Id <= 0)
        {
            return BadRequest("Id must be greater than zero when deleting an item.");
        }

        if (request.ItemState != ItemState.Delete && string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Name is required for adding or updating an item.");
        }

        var result = await itemService.ProcessAsync(request, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        if (result.WasDeleted)
        {
            return NoContent();
        }

        if (request.ItemState == ItemState.Add
            || request.ItemState == ItemState.AddOrEdit && request.Id == 0)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Item!.Id },
                result.Item);
        }

        return Ok(result.Item);
    }
}
