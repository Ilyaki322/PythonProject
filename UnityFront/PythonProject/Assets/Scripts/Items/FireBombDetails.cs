using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Fire Bomb")]
public class FireBombDetails : ItemDetails
{
    public int damage = 15;

    public override void Use(CharacterCombatController user, CharacterCombatController opponent)
    {
        opponent.TakeDamage(damage);
    }
}
