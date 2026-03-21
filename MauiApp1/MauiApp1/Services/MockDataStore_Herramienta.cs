using iAlmacen.Models;

namespace iAlmacen.Services;

public class MockDataStore_Herramienta : IDataStore_Herramienta<Item_RegArticulo>
{
    private List<Item_RegArticulo> items;

    public MockDataStore_Herramienta()
    {
        items = new List<Item_RegArticulo>();
        var mockItems = new List<Item_RegArticulo>
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

    public async Task<bool> AddItemAsync(Item_RegArticulo item)
    {
        items.Add(item);

        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateItemAsync(Item_RegArticulo item)
    {
        var _item = items.Where((Item_RegArticulo arg) => arg.id == item.id).FirstOrDefault();
        items.Remove(_item);
        items.Add(item);

        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var _item = items.Where((Item_RegArticulo arg) => arg.id == id).FirstOrDefault();
        items.Remove(_item);

        return await Task.FromResult(true);
    }

    public async Task<Item_RegArticulo> GetItemAsync(int id)
    {
        return await Task.FromResult(items.FirstOrDefault(s => s.id == id));
    }

    public async Task<IEnumerable<Item_RegArticulo>> GetItemsAsync(bool forceRefresh = false)
    {
        return await Task.FromResult(items);
    }
}