using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class CharacterCombatController : MonoBehaviour
{
    [SerializeField] Animator m_animator;

    private CharacterCombatController m_enemyController;
    private GameController m_gameController;

    private Label m_healthLabel;
    private VisualElement m_healthFill;

    private int m_maxHealth;
    private int m_currentHealth;

    private bool m_def;

    private readonly int ATTACK_DAMAGE = 10;

    public void Init(Label healthLabel, VisualElement healthFill, 
        CharacterCombatController enemy, GameController gameController)
    {
        m_gameController = gameController;
        m_enemyController = enemy;

        m_maxHealth = 100;
        m_currentHealth = 100;

        m_healthLabel = healthLabel;
        m_healthFill = healthFill;

        m_healthLabel.text = "100/100";
        m_healthFill.style.width = Length.Percent(100);
    }

    public void TakeDamage(int damage)
    {
        if (m_def)
        {
            m_def = false;
            m_gameController.TurnShieldOff();
            return;
        }

        m_currentHealth -= damage;
        if (m_currentHealth < 0)
        {

        }

        float healthRatio = (float)m_currentHealth / m_maxHealth;
        float healthPercent = Mathf.Lerp(0, 100, healthRatio);
        m_healthFill.style.width = Length.Percent(healthPercent);
        m_healthLabel.text = m_currentHealth.ToString() + "/100";
    }

    public void OnAttack()
    {
        m_enemyController.TakeDamage(ATTACK_DAMAGE);
        m_animator.SetTrigger("Attack");
        m_animator.SetFloat("AttackState", 0.0f);
        m_animator.SetFloat("NormalState", 0.0f);
    }

    public void OnDefend()
    {
        m_gameController.TurnShieldOn();
        m_def = true;
        m_animator.SetTrigger("Attack");
        m_animator.SetFloat("AttackState", 1.0f);
        m_animator.SetFloat("NormalState", 0.0f);
    }

    public void OnItemUse()
    {
        Debug.Log("USE ITEM!");

    }

    public void OnFourthButton()
    {
        Debug.Log("????!");
        m_animator.SetTrigger("Die");
        m_animator.SetBool("EditChk", true);
    }
}
