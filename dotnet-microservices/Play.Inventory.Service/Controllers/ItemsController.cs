using System.Net;
using Microsoft.AspNetCore.Mvc;
using Play.Common;

[ApiController]
[Route("items")]
public class ItemsController
{
  private readonly IRepository<InventoryItem> itemsRepository;
  private readonly CatalogClient catalogClient;

  public ItemsController(IRepository<InventoryItem> itemsRepository, CatalogClient catalogClient)
  {
    this.itemsRepository = itemsRepository;
    this.catalogClient = catalogClient;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
  {
    if (userId == Guid.Empty)
    {
      return new BadRequestResult();
    }
    var catalogItems = await catalogClient.GetCatalogItemsAsync();
    var inventoryItemEntities = await itemsRepository.GetAllAsync(item => item.UserId == userId);
    var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
    {
      var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
      return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
    });
    return new OkObjectResult(inventoryItemDtos);
  }

  [HttpPost]
  public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
  {
    var inventoryItem = await itemsRepository.GetAsync(item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

    if (inventoryItem == null)
    {
      inventoryItem = new InventoryItem
      {
        CatalogItemId = grantItemsDto.CatalogItemId,
        UserId = grantItemsDto.UserId,
        Quantity = grantItemsDto.Quantity,
        AcquiredDate = DateTimeOffset.UtcNow
      };

      await itemsRepository.CreateAsync(inventoryItem);
    }
    else
    {
      inventoryItem.Quantity += grantItemsDto.Quantity;
      await itemsRepository.UpdateAsync(inventoryItem);
    }

    return new OkResult();
  }
}