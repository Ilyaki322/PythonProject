using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
public class ShopController : MonoBehaviour
{
    [SerializeField] GameController m_gameController;
    [SerializeField] InventoryView m_shopInventoryView;
    [SerializeField] InventoryView m_userInventoryView;

    [Header("Shop Items")]
    [SerializeField] List<ItemDetails> m_itemsList;

    private VisualElement m_root;
    private VisualElement m_shopContainer;
    private VisualElement m_userInventory;
    private VisualElement m_shopInventory;

    private Button m_levelUpButton;
    private Button m_backButton;

    private void Awake()
    {
        m_root = GetComponent<UIDocument>().rootVisualElement;
        m_userInventory = m_root.Q<VisualElement>("UserInventory");
        m_shopInventory = m_root.Q<VisualElement>("ShopInventory");
        m_shopContainer = m_root.Q<VisualElement>("Shop");
        m_levelUpButton = m_root.Q<Button>("LevelUpButton");
        m_backButton = m_root.Q<Button>("BackButton");

        m_backButton.clicked += HideShop;
    }

    private void Start()
    {
        // Build the shop inventory (you might pass real shop items here)
        new InventoryController.Builder(m_shopInventoryView)
            .WithCapacity(10)
            .WithContainer(m_shopInventory)
            .WithStartingItems(m_itemsList)
            .Build();

       // Build the player’s inventory
        new InventoryController.Builder(m_userInventoryView)
            .WithCapacity(10)
            .WithContainer(m_userInventory)
            //.WithApi(playerInventoryApi)            // if loading from API
            .Build();

        //m_userInventoryView.InitView();
        //m_shopInventoryView.InitView();
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
}
