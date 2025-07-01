using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemDetails : ScriptableObject
{
    public string Name;
    public int maxStack = 1;
    public SerializableGuid Id = SerializableGuid.NewGuid();

    public Sprite Icon;

    public string Description;

    public Item Create(int qty)
    {
        return new Item(this, qty);
    }
}
