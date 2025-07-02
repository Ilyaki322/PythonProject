using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] InventoryView m_view;
    [SerializeField] int m_capacity;
    [SerializeField] List<ItemDetails> m_startingItems = new List<ItemDetails>();

    InventoryController m_controller;

    public void LoadInventory(InventoryApi api)
    {
        m_controller = new InventoryController.Builder(m_view)
            .WithApi(api)
            .WithCapacity(m_capacity)
            .Build();

        m_view.InitView();
    }
}
