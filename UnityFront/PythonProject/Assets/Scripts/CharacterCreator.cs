using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    [SerializeField] private Sprite[] m_hair;
    [SerializeField] private Sprite[] m_beard;
    [SerializeField] private Sprite[] m_helm;
    [SerializeField] private ArmorSpriteSO[] m_armor;
    [SerializeField] private LegSpriteSO[] m_pants;
    [SerializeField] private Sprite[] m_weapon;
    [SerializeField] private Sprite[] m_back;

    [SerializeField, Space(10)] private Sprite m_nakedLeftArm;
    [SerializeField, Space(10)] private Sprite m_nakedRightArm;
    [SerializeField, Space(10)] private Sprite m_nakedBody;
    [SerializeField, Space(10)] private Sprite m_nakedLeftFoot;
    [SerializeField, Space(10)] private Sprite m_nakedRightFoot;
    [SerializeField, Space(10)] private Sprite m_nakedHead;

    [Space(10)]
    [SerializeField] private SpriteRenderer m_headSlot;
    [SerializeField] private SpriteRenderer m_beardSlot;
    [SerializeField] private SpriteRenderer m_helmSlot;

    [SerializeField] private SpriteRenderer m_armorSlot;
    [SerializeField] private SpriteRenderer m_shoulderLeft;
    [SerializeField] private SpriteRenderer m_shoulderRight;

    [SerializeField] private SpriteRenderer m_leftLegSlot;
    [SerializeField] private SpriteRenderer m_rightLegSlot;

    [SerializeField] private SpriteRenderer m_weapon1;
    [SerializeField] private SpriteRenderer m_weapon2;

    [SerializeField] private SpriteRenderer m_backSlot;

    [Space(10), SerializeField] private CharacterSelectionController m_characterSelectionController;

    private int m_hairIndex = 0;
    private int m_beardIndex = 0;
    private int m_helmIndex = 0;
    private int m_armorIndex = 0;
    private int m_pantsIndex = 0;
    private int m_weapon1Index = 0;
    private int m_weapon2Index = 0;
    private int m_backIndex = 0;

    private void Change(int change, ref int index, Sprite[] spr, SpriteRenderer slot, Sprite naked)
    {
        if (spr == null || spr.Length == 0) return;

        int i = (index + change) % spr.Length;
        if (i == -1) i = spr.Length - 1;
        if (i == 0)
        {
            slot.sprite = naked;
            index = 0;
        }
        else
        {
            slot.sprite = spr[i];
            index = i;
        }
    }

    public void ChangeHair(int change)
    {
        if (m_hair == null || m_hair.Length == 0) return;

        int i = (m_hairIndex + change) % m_hair.Length;
        if (i == -1) i = m_hair.Length - 1;
        if (i == 0)
        {
            m_hairIndex = 0;
            m_headSlot.sprite = null;
        }
        else
        {
            m_hairIndex = i;
            m_headSlot.sprite = m_hair[i];
        }

        m_helmIndex = 0;
        m_helmSlot.sprite = null;
    }

    public void ChangeHelm(int change)
    {
        if (m_helm == null || m_helm.Length == 0) return;

        int i = (m_helmIndex + change) % m_helm.Length;
        if (i == -1) i = m_helm.Length - 1;
        if (i == 0)
        {
            m_helmIndex = 0;
            m_helmSlot.sprite = null;
        }
        else
        {
            m_helmIndex = i;
            m_helmSlot.sprite = m_helm[i];
        }

        m_hairIndex = 0;
        m_headSlot.sprite = null;
    }

    public void ChangeArmor(int change)
    {
        if (m_armor == null || m_armor.Length == 0) return;

        int i = (m_armorIndex + change) % m_armor.Length;
        if (i == -1) i = m_armor.Length - 1;
        if (i == 0)
        {
            m_armorIndex = 0;
            m_armorSlot.sprite = m_nakedBody;
            m_shoulderLeft.sprite = null;
            m_shoulderRight.sprite = null;
        }
        else
        {
            m_armorIndex = i;
            m_armorSlot.sprite = m_armor[i].Armor;
            m_shoulderLeft.sprite = m_armor[i].LShoulder;
            m_shoulderRight.sprite = m_armor[i].RShoulder;
        }
    }

    public void ChangePants(int change)
    {
        if (m_pants == null || m_pants.Length == 0) return;

        int i = (m_pantsIndex + change) % m_pants.Length;
        if (i == -1) i = m_pants.Length - 1;
        if (i == 0)
        {
            m_pantsIndex = 0;
            m_leftLegSlot.sprite = m_nakedLeftFoot;
            m_rightLegSlot.sprite = m_nakedRightFoot;
        }
        else
        {
            m_pantsIndex = i;
            m_leftLegSlot.sprite = m_pants[i].LLeg;
            m_rightLegSlot.sprite = m_pants[i].RLeg;
        }
    }


    public void ChangeBeard(int change) => Change(change, ref m_beardIndex, m_beard, m_beardSlot, null);
    public void ChangeBack(int change) => Change(change, ref m_backIndex, m_back, m_backSlot, null);
    public void ChangeWeapon1(int change) => Change(change, ref m_weapon1Index, m_weapon, m_weapon1, null);
    public void ChangeWeapon2(int change) => Change(change, ref m_weapon2Index, m_weapon, m_weapon2, null);

    public void Generate(CharacterDTO character)
    {
        m_hairIndex = character.hair;
        m_beardIndex = character.beard;
        m_helmIndex = character.helmet;
        m_armorIndex = character.armor;
        m_pantsIndex = character.pants;
        m_weapon1Index = character.weapon;
        m_weapon2Index = 0;
        m_backIndex = character.back;

        if (m_hairIndex != 0) ChangeHair(0);
        else ChangeHelm(0);
        
        ChangeBeard(0);
        ChangeArmor(0);
        ChangePants(0);
        ChangeWeapon1(0);
        ChangeBack(0);
    }

    public void save(string name)
    {
        CharacterDTO character = new CharacterDTO();
        character.weapon = m_weapon1Index;
        character.back = m_backIndex;
        character.armor = m_armorIndex;
        character.beard = m_beardIndex;
        character.helmet = m_helmIndex;
        character.hair = m_hairIndex;
        character.pants = m_pantsIndex;
        character.name = name;

        m_characterSelectionController.Save(character);
    }
}
