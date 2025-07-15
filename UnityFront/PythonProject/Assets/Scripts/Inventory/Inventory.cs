using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    [SerializeField] InventoryView m_view;
    [SerializeField] int m_capacity;
    [SerializeField] List<ItemDetails> m_startingItems = new List<ItemDetails>();
    [SerializeField] UIDocument gameMenu;
    InventoryController m_controller;

    public void Start()
    {
        m_controller = new InventoryController.Builder(m_view)
            .WithContainer(gameMenu.rootVisualElement.Q<VisualElement>("Hui"))
            .WithStartingItems(m_startingItems)
            .WithCapacity(m_capacity)
            .Build();

        m_view.InitView();
    }
}
