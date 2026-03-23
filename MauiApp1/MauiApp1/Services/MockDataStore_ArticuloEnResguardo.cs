using iAlmacen.Models;

namespace iAlmacen.Services;

public class MockDataStore_ArticuloEnResguardo : IDataStore_Herramienta<Item_ArticuloEnResguardo>
{
    private List<Item_ArticuloEnResguardo> items;

    public MockDataStore_ArticuloEnResguardo()
    {
        items = new List<Item_ArticuloEnResguardo>();
        var mockItems = new List<Item_ArticuloEnResguardo>
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

    public async Task<bool> AddItemAsync(Item_ArticuloEnResguardo item)
    {
        items.Add(item);

        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateItemAsync(Item_ArticuloEnResguardo item)
    {
        var _item = items.Where((Item_ArticuloEnResguardo arg) => arg.id == item.id).FirstOrDefault();
        items.Remove(_item);
        items.Add(item);

        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var _item = items.Where((Item_ArticuloEnResguardo arg) => arg.id == id).FirstOrDefault();
        items.Remove(_item);

        return await Task.FromResult(true);
    }

    public async Task<Item_ArticuloEnResguardo> GetItemAsync(int id)
    {
        return await Task.FromResult(items.FirstOrDefault(s => s.id == id));
    }

    public async Task<IEnumerable<Item_ArticuloEnResguardo>> GetItemsAsync(bool forceRefresh = false)
    {
        return await Task.FromResult(items);
    }
}