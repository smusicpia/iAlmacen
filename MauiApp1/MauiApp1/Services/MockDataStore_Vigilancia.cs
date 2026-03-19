using iAlmacen.Models;

namespace iAlmacen.Services
{
    public class MockDataStore_Vigilancia : IDataStore_Vigilancia<Item_entrada_vigilancia>
    {
        private List<Item_entrada_vigilancia> items;

        public MockDataStore_Vigilancia()
        {
            items = new List<Item_entrada_vigilancia>();
            var mockItems = new List<Item_entrada_vigilancia>
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

        public async Task<bool> AddItemAsync(Item_entrada_vigilancia item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item_entrada_vigilancia item)
        {
            var _item = items.Where((Item_entrada_vigilancia arg) => arg.id_ == item.id_).FirstOrDefault();
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(float id)
        {
            var _item = items.Where((Item_entrada_vigilancia arg) => arg.id_ == id).FirstOrDefault();
            items.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<Item_entrada_vigilancia> GetItemAsync(float id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.id_ == id));
        }

        public async Task<IEnumerable<Item_entrada_vigilancia>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}