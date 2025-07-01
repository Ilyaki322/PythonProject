using System;
using System.Collections.Generic;

public class InventoryModel
{
    public ObservableArray<Item> Items;

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

    public Item Get(int index) => Items[index];
    public void Clear() => Items.Clear();
    public bool Add(Item item) => Items.TryAdd(item);
    public bool Remove(Item item) => Items.TryRemove(item);

    public void Swap(int source, int target) => Items.Swap(source, target);

    public int Combine(int source, int target)
    {
        var total = Items[source].Quantity + Items[target].Quantity;
        Items[target].Quantity = total;
        Remove(Items[source]);
        return total;
    }
}
