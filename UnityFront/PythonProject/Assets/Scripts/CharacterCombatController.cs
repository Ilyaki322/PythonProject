using UnityEngine;
using UnityEngine.UIElements;
using static InventoryModel;
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

    private readonly int BASE_HEALTH = 100;
    private readonly int ATTACK_DAMAGE = 10;
    private int m_attack;

    public void Init(Label healthLabel, VisualElement healthFill, 
        CharacterCombatController enemy, GameController gameController)
    {
        m_animator.SetBool("EditChk", true);
        m_gameController = gameController;
        m_enemyController = enemy;

        m_def = false;

        m_healthLabel = healthLabel;
        m_healthFill = healthFill;

        m_healthLabel.text = "100/100";
        m_healthFill.style.width = Length.Percent(100);
    }

    public void SetPlayer(CharacterDTO player)
    {
        m_maxHealth = player.CharLevel + BASE_HEALTH;
        m_currentHealth = m_maxHealth;

        m_attack = player.CharLevel + ATTACK_DAMAGE;
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
        if (m_currentHealth <= 0)
        {
            m_animator.SetTrigger("Die");
            m_animator.SetBool("EditChk", false);
            m_gameController.OnDead();
        }

        float healthRatio = (float)m_currentHealth / m_maxHealth;
        float healthPercent = Mathf.Lerp(0, 100, healthRatio);
        m_healthFill.style.width = Length.Percent(healthPercent);
        m_healthLabel.text = m_currentHealth.ToString() + "/100";
    }

    public int OnAttack()
    {
        int minDamage = Mathf.Max(0, m_attack - 2);
        int maxDamage = m_attack + 2;
        int damage = Random.Range(minDamage, maxDamage + 1);

        m_enemyController.TakeDamage(damage);

        m_animator.SetTrigger("Attack");
        m_animator.SetFloat("AttackState", 0.0f);
        m_animator.SetFloat("NormalState", 0.0f);

        return damage;
    }

    public void OnAttack(int damage)
    {
        m_enemyController.TakeDamage(damage);

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

    public void OnItemUse(int index, SerializableGuid itemdId)
    {
       var item = ItemDatabase.Instance.GetItemDetailsById(itemdId);
        if (item)
        {
            item.Use(this, m_enemyController);
            m_gameController.RemoveItem(index);
        }
    }

    public void OnItemUse(string itemId)
    {
        var item = ItemDatabase.Instance.GetItemDetailsById(itemId);
        if (item)
        {
            item.Use(this, m_enemyController);
        }
    }

    public void OnFourthButton()
    {
        m_animator.SetTrigger("Die");
        m_animator.SetBool("EditChk", false);
    }

    public void OnHeal(int healAmount) 
    {
        m_currentHealth += healAmount;
        if (m_currentHealth > m_maxHealth) 
        {
            m_currentHealth = m_maxHealth;
        }

        float healthRatio = (float)m_currentHealth / m_maxHealth;
        float healthPercent = Mathf.Lerp(0, 100, healthRatio);
        m_healthFill.style.width = Length.Percent(healthPercent);
        m_healthLabel.text = m_currentHealth.ToString() + "/100";
    }
}
