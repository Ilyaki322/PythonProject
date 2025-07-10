using UnityEngine;

public class CharacterCombatController : MonoBehaviour
{
    public void OnAttack()
    {
        Debug.Log("ATTACK!");
    }

    public void OnDefend()
    {
        Debug.Log("DEFEND!");
    }

    public void OnItemUse()
    {
        Debug.Log("USE ITEM!");
    }

    public void OnFourthButton()
    {
        Debug.Log("????!");
    }
}
