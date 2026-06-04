using iAlmacen.Models;

namespace iAlmacen.Services;

public class MockDataStore_SalidaArticulo : IDataStore_SalidaArticulo<Item_ArticuloSalida>
{
    private List<Item_ArticuloSalida> items;

    public MockDataStore_SalidaArticulo()
    {
        items = new List<Item_ArticuloSalida>();
        var mockItems = new List<Item_ArticuloSalida>
        {
            //new Item { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." },
            //new Item { Id = Guid.NewGuid().ToString(), Text = "Second item", Description="This is an item description." },
            //new Item { Id = Guid.NewGuid().ToString(), Text = "Third item", Description="This is an item description." },
            //new Item { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description="This is an item description." },
            //new Item { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description="This is an item description." },
            //new Item { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description." },
        };

        foreach (var item in mockItems)
        {
            items.Add(item);
        }
    }

    public async Task<bool> AddItemAsync(Item_ArticuloSalida item)
    {
        items.Add(item);

        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateItemAsync(Item_ArticuloSalida item)
    {
        var _item = items.Where((Item_ArticuloSalida arg) => arg.id == item.id).FirstOrDefault();
        items.Remove(_item);
        items.Add(item);

        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var _item = items.Where((Item_ArticuloSalida arg) => arg.id == id).FirstOrDefault();
        items.Remove(_item);

        return await Task.FromResult(true);
    }

    public async Task<Item_ArticuloSalida> GetItemAsync(int id)
    {
        return await Task.FromResult(items.FirstOrDefault(s => s.id == id));
    }

    public async Task<IEnumerable<Item_ArticuloSalida>> GetItemsAsync(bool forceRefresh = false)
    {
        return await Task.FromResult(items);
    }
}