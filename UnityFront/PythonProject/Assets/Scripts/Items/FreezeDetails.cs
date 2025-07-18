using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Freeze")]
public class FreezeDetails : ItemDetails
{
    public int damage = 7;

    public override void Use(CharacterCombatController user, CharacterCombatController opponent)
    {
        opponent.TakeDamage(damage);
    }
}