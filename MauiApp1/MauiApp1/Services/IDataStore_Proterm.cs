namespace iAlmacen.Services;

public interface IDataStore_Proterm<T>
{
    Task<bool> AddItemAsync(T item);

    Task<bool> UpdateItemAsync(T item);

    Task<bool> DeleteItemAsync(float id);

    Task<T> GetItemAsync(float id);

    Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
}