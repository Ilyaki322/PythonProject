using System;
using System.Collections.Generic;
using System.Linq;
public class InventoryModel
{
    public ObservableArray<Item> Items;

    InventoryApi m_inventoryApi;

    public event Action<Item[]> OnModelChanged
    {
        add => Items.AnyValueChanged += value;
        remove => Items.AnyValueChanged -= value;
    }

    public InventoryModel(IEnumerable<ItemDetails> itemDetails, int capacity)
    {
        Items = new ObservableArray<Item>(capacity);
        foreach (var item in itemDetails)
        {
            Items.TryAdd(item.Create(1));
        }
    }

    public InventoryModel(int capacity, InventoryApi inventoryApi)
    {
        Items = new ObservableArray<Item>(capacity);
        m_inventoryApi = inventoryApi;
        inventoryApi.StartCoroutine(inventoryApi.GetItems(loadFromApi));
    }

    public List<(Item, int)> GetAll()
    {
        var list = new List<(Item, int)>();
        for (int i = 0; i < Items.Length; i++)
        {
            var item = Items[i];
            if (item != null)
                list.Add((item, i));
        }

        return list;
    }

    public Item Get(int index) => Items[index];
    public void Clear() => Items.Clear();
    public bool Add(Item item) => Items.TryAdd(item);

    public bool TryAddAt(int index, Item item) => Items.TryAddAt(index, item);
    public bool Remove(Item item) => Items.TryRemove(item);

    public void Swap(int source, int target) => Items.Swap(source, target);

    public int Combine(int source, int target)
    {
        var total = Items[source].Quantity + Items[target].Quantity;
        Items[target].Quantity = total;
        Remove(Items[source]);
        return total;
    }


    private void loadFromApi(string json)
    {
        InventoryEntryDTO[] entries = JsonHelper.FromJson<InventoryEntryDTO>(json);

        foreach (var entry in entries)
        {
            Items.TryAddAt(entry.index, m_inventoryApi.getByID(entry.item.id).Create(entry.count));
        }
    }

    [System.Serializable]
    public class InventoryEntryDTO
    {
        public ItemDTO item;
        public int count;
        public int index;
    }

    [System.Serializable]
    public class ItemDTO
    {
        public string id;
        public string name;
        public string description;
    }
}
