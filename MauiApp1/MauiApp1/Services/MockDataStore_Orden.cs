using iAlmacen.Models;

namespace iAlmacen.Services
{
    public class MockDataStore_Orden : IDataStore_Orden<Item_orden_compra>
    {
        private List<Item_orden_compra> items;

        public MockDataStore_Orden()
        {
            items = new List<Item_orden_compra>();
            var mockItems = new List<Item_orden_compra>
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

        public async Task<bool> AddItemAsync(Item_orden_compra item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item_orden_compra item)
        {
            var _item = items.Where((Item_orden_compra arg) => arg.id_orden_ == item.id_orden_).FirstOrDefault();
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(float id)
        {
            var _item = items.Where((Item_orden_compra arg) => arg.id_orden_ == id).FirstOrDefault();
            items.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<Item_orden_compra> GetItemAsync(float id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.id_orden_ == id));
        }

        public async Task<IEnumerable<Item_orden_compra>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}