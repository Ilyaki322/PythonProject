using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Healing Potion")]
public class HealingPotionDetails : ItemDetails
{
    public int healAmount = 20;

    public override void Use(CharacterCombatController user, CharacterCombatController opponent)
    {
        user.OnHeal(healAmount);
    }
}
