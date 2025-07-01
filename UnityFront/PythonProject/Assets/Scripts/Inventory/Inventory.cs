using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] InventoryView m_view;
    [SerializeField] int m_capacity;
    [SerializeField] List<ItemDetails> m_startingItems = new List<ItemDetails>();

    InventoryController m_controller;

    private void Awake()
    {
        m_controller = new InventoryController.Builder(m_view)
            .WithStartingItems(m_startingItems)
            .WithCapacity(m_capacity)
            .Build();
    }
}
