using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Scratch")]
public class ScratchDetails : ItemDetails
{
    public int damage = 5;

    public override void Use(CharacterCombatController user, CharacterCombatController opponent)
    {
        opponent.TakeDamage(damage);
    }
}
