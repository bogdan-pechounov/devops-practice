using Microsoft.AspNetCore.Mvc;
using Play.Common;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
  private readonly IRepository<Item> itemsRepository;

  public ItemsController(IRepository<Item> itemsRepository)
  {
    this.itemsRepository = itemsRepository;
  }

  [HttpGet]
  public async Task<IEnumerable<ItemDto>> GetAsync()
  {
    var items = await itemsRepository.GetAllAsync();
    var itemsDto = items.Select(item => item.AsDto());
    return itemsDto;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
  {
    var item = await itemsRepository.GetAsync(id);
    if (item == null) return NotFound();
    return item.AsDto();
  }

  [HttpPost]
  public async Task<ActionResult<ItemDto>> Post(CreateItemDto createItemDto)
  {
    var item = new Item
    {
      Name = createItemDto.Name,
      Description = createItemDto.Description,
      Price = createItemDto.Price,
      CreatedDate = DateTimeOffset.UtcNow
    };
    await itemsRepository.CreateAsync(item);
    return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
  {
    var existingItem = await itemsRepository.GetAsync(id);
    if (existingItem == null) return NotFound();

    existingItem.Name = updateItemDto.Name;
    existingItem.Description = updateItemDto.Description;
    existingItem.Price = updateItemDto.Price;

    await itemsRepository.UpdateAsync(existingItem);

    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteAsync(Guid id)
  {
    var item = await itemsRepository.GetAsync(id);
    if (item == null) return NotFound();
    await itemsRepository.RemoveAsync(item.Id);
    return NoContent();
  }
}