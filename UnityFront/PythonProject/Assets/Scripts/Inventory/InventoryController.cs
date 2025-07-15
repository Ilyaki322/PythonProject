using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class InventoryController
{
    readonly InventoryModel m_model;
    readonly InventoryView m_view;
    readonly int m_capacity;

    InventoryController(InventoryModel model, InventoryView view, int capacity)
    {
        Debug.Assert(model != null, "model is null");
        Debug.Assert(view != null, "View is null");
        Debug.Assert(capacity > 0, "Capacity is less than 1");
        m_model = model;
        m_view = view;
        m_capacity = capacity;

        m_view.StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        yield return m_view.InitializeView(m_capacity);

        m_view.OnDrop += HandleDrop;
        m_model.OnModelChanged += HandleModelChanged;

        RefreshView();
    }

    void HandleDrop(Slot originalSlot, Slot closestSlot)
    {
        if (originalSlot.Index == closestSlot.Index || closestSlot.ItemId.Equals(SerializableGuid.Empty))
        {
            m_model.Swap(originalSlot.Index, closestSlot.Index);
            return;
        }

        var sourceItemId = m_model.Items[originalSlot.Index].Details.Id;
        var targetItemId = m_model.Items[closestSlot.Index].Details.Id;

        if (sourceItemId.Equals(targetItemId) && 
            m_model.Items[closestSlot.Index].Quantity + m_model.Items[originalSlot.Index].Quantity <= m_model.Items[closestSlot.Index].Details.maxStack)
        {
            m_model.Combine(originalSlot.Index, closestSlot.Index);
        }
        else
        {
            m_model.Swap(originalSlot.Index, closestSlot.Index);
        }
    }

    void HandleModelChanged(IList<Item> items) => RefreshView();

    void RefreshView()
    {
        for (int i = 0; i < m_capacity; i++)
        {
            var item = m_model.Get(i);
            if (item == null || item.Id.Equals(SerializableGuid.Empty))
            {
                m_view.Slots[i].Set(SerializableGuid.Empty, null);
            }
            else
            {
                m_view.Slots[i].Set(item.Id, item.Details.Icon, item.Quantity);
            }
        }
    }

    #region Builder

    public class Builder
    {
        InventoryView view;
        IEnumerable<ItemDetails> itemDetails;
        int capacity = 20;

        bool fromAPI = false;
        InventoryApi inventoryApi;

        public Builder(InventoryView view)
        {
            this.view = view;
        }

        public Builder WithStartingItems(IEnumerable<ItemDetails> items)
        {
            itemDetails = items;
            return this;
        }

        public Builder WithApi(InventoryApi inventoryApi)
        {
            this.inventoryApi = inventoryApi;
            fromAPI = true;
            return this;
        }

        public Builder WithCapacity(int cap)
        {
            capacity = cap;
            return this;
        }

        public InventoryController Build()
        {
            InventoryModel model;

            if (fromAPI)
            {
                model = new InventoryModel(capacity, inventoryApi);
            }
            else
            {
                model = itemDetails != null
                ? new InventoryModel(itemDetails, capacity)
                : new InventoryModel(Array.Empty<ItemDetails>(), capacity);
            }
            
            return new InventoryController(model, view, capacity);
        }

        public Builder WithContainer(VisualElement container)
        {
            view.SetInventoryHolder(container);
            return this;
        }
    }

    #endregion
}
