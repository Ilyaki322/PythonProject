using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.ComponentModel;
using System;
public class ShopController : MonoBehaviour
{
    [SerializeField] GameController m_gameController;

    public event Action<string, int, int> OnPurchase;
    public event Action<string, int, int> OnSale;
    public event Action<int> OnLevelUp;

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
            if (m_selectedCharacter != null)
                m_selectedCharacter.PropertyChanged -= OnCharacterPropertyChanged;

            m_selectedCharacter = value;

            if (m_selectedCharacter != null)
            {
                m_selectedCharacter.PropertyChanged += OnCharacterPropertyChanged;
                // Initialize button text for current level
                UpdateLevelUpText(m_selectedCharacter.level);
            }
        }
    }

    private void Awake()
    {
        m_root = GetComponent<UIDocument>().rootVisualElement;
        m_shopContainer = m_root.Q<VisualElement>("Shop");
        m_levelUpButton = m_root.Q<Button>("LevelUpButton");
        
        if(m_selectedCharacter != null)
        {
            m_levelUpButton.text = $"Level Up ({m_selectedCharacter.level + 1})";
        }

        m_backButton = m_root.Q<Button>("BackButton");

        m_backButton.clicked += HideShop;
        m_levelUpButton.clicked += OnLevelUpButton;
    }

    private void OnCharacterPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CharacterDTO.level))
            UpdateLevelUpText(m_selectedCharacter.level);
    }

    private void UpdateLevelUpText(int level)
    {
        m_levelUpButton.text = $"Level Up ({level + 1})";
    }

    private void OnLevelUpButton()
    {
        if (m_selectedCharacter == null)
        {
            Debug.LogWarning("No character selected for level up.");
            return;
        }

        int levelUpCost = m_selectedCharacter.level + 1; //cost calculation

        if (LevelUpCheck(levelUpCost))
        {
            m_selectedCharacter.level++;
            Debug.Log($"Character {m_selectedCharacter.name} leveled up to level {m_selectedCharacter.level}.");
            OnLevelUp?.Invoke(m_selectedCharacter.level);
        }
        else
        {
            Debug.LogWarning("Not enough money to level up.");
        }
    }

    bool LevelUpCheck(int levelUpCost)
    {
        if (m_selectedCharacter.money >= levelUpCost)
        {
            m_selectedCharacter.money -= levelUpCost;
            return true;
        }
        return false;
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

    public bool makePurchase(Item item, int slotIndex)
    {
        if (m_selectedCharacter.money >= item.Details.Price)
        {
            m_selectedCharacter.money -= item.Details.Price;
            OnPurchase?.Invoke(item.Details.Id.ToHexString(), item.Quantity, slotIndex);
            return true;
        }
        return false;
    }

    public bool sellItem(Item item, int slotIndex)
    {
        if (m_selectedCharacter.money + item.Details.Price <= int.MaxValue)
        {
            m_selectedCharacter.money += item.Details.Price;
            OnSale?.Invoke(item.Details.Id.ToHexString(), item.Quantity, slotIndex);
            return true;
        }
        return false;
    }
}
