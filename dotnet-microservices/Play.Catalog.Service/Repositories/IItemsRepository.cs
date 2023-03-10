public interface IItemsRepository
{
  Task<IReadOnlyCollection<Item>> GetAllAsync();
  Task<Item> GetAsync(Guid id);
  Task CreateAsync(Item entity);
  Task UpdateAsync(Item entity);
  Task RemoveAsync(Guid id);
}
