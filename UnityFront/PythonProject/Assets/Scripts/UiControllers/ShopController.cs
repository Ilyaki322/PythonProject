using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
public class ShopController : MonoBehaviour
{
    [SerializeField] GameController m_gameController;


    private VisualElement m_root;
    private VisualElement m_shopContainer;

    private Button m_levelUpButton;
    private Button m_backButton;

    private CharacterDTO m_selectedCharacter;

    public CharacterDTO SelectedCharacter
    {
        get => m_selectedCharacter;
        set
        {
            m_selectedCharacter = value;
        }
    }

    private void Awake()
    {
        m_root = GetComponent<UIDocument>().rootVisualElement;
        m_shopContainer = m_root.Q<VisualElement>("Shop");
        m_levelUpButton = m_root.Q<Button>("LevelUpButton");
        m_backButton = m_root.Q<Button>("BackButton");

        m_backButton.clicked += HideShop;
    }

    public void HideShop()
    {
        m_shopContainer.style.display = DisplayStyle.None;
        m_gameController.ShowMenu();
    }

    public void ShowShop()
    {
        m_shopContainer.style.display = DisplayStyle.Flex;
    }

    public bool makePurchase(int cost)
    {
        if (m_selectedCharacter.money >= cost)
        {
            m_selectedCharacter.money -= cost;
            return true;
        }
        return false;
    }

    public bool sellItem(int value)
    {
        if (m_selectedCharacter.money + value <= int.MaxValue)
        {
            m_selectedCharacter.money += value;
            return true;
        }
        return false;
    }
}
