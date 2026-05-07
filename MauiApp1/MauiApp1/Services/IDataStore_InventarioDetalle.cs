namespace iAlmacen.Services;

public interface IDataStore_InventarioDetalle<T> where T : class
{
    Task<bool> AddItemAsync(T item);

    Task<bool> UpdateItemAsync(T item);

    Task<bool> DeleteItemAsync(int id);

    Task<T> GetItemAsync(int id);

    Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
}