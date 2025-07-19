using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public abstract class ItemDetails : ScriptableObject
{
    public string Name;
    public int maxStack = 1;
    public SerializableGuid Id = SerializableGuid.NewGuid();

    public Sprite Icon;

    public int Price = 0;
    public string Description;
    public string OnUse;

    public Item Create(int qty)
    {
        return new Item(this, qty);
    }

    public abstract void Use(CharacterCombatController user, CharacterCombatController opponent);
}