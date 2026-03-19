using iAlmacen.Models;

namespace iAlmacen.Services;

public class MockDataStore_Recoleccion : iDataStore_Recoleccion<Item_Virtual_Recoleccion>
{
    private List<Item_Virtual_Recoleccion> items;

    public MockDataStore_Recoleccion()
    {
        items = new List<Item_Virtual_Recoleccion>();
        var mockItems = new List<Item_Virtual_Recoleccion>
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

    public async Task<bool> AddItemAsync(Item_Virtual_Recoleccion item)
    {
        items.Add(item);

        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteItemAsync(string id)
    {
        var _item = items.Where((Item_Virtual_Recoleccion arg) => arg.folio_orden_ == id).FirstOrDefault();
        items.Remove(_item);

        return await Task.FromResult(true);
    }

    public async Task<Item_Virtual_Recoleccion> GetItemAsync(string id)
    {
        return await Task.FromResult(items.FirstOrDefault(s => s.folio_orden_ == id));
    }

    public async Task<IEnumerable<Item_Virtual_Recoleccion>> GetItemsAsync(bool forceRefresh = false)
    {
        return await Task.FromResult(items);
    }

    public async Task<bool> UpdateItemAsync(Item_Virtual_Recoleccion item)
    {
        var _item = items.Where((Item_Virtual_Recoleccion arg) => arg.folio_orden_ == item.folio_orden_).FirstOrDefault();
        items.Remove(_item);
        items.Add(item);

        return await Task.FromResult(true);
    }
}