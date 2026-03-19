namespace iAlmacen.Services;

public interface iDataStore_Recoleccion<T>
{
    Task<bool> AddItemAsync(T item);

    Task<bool> UpdateItemAsync(T item);

    Task<bool> DeleteItemAsync(string id);

    Task<T> GetItemAsync(string id);

    Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
}