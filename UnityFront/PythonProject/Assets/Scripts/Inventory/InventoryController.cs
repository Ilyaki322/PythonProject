using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public enum InventoryRole { Shop, User }

public class InventoryController
{
    readonly InventoryModel m_model;
    readonly InventoryView m_view;
    readonly int m_capacity;

    InventoryRole m_role = InventoryRole.User;
    Func<ItemDetails, bool> m_onBuy = _ => true;
    Func<Item, bool> m_onSell = _ => true;
    InventoryController m_other;


    public void SetRole(InventoryRole role) => m_role = role;
    public void RegisterShopHandler(Func<ItemDetails, bool> onBuy) => m_onBuy = onBuy;

    public void RegisterUserHandler(Func<Item, bool> onSell) => m_onSell = onSell;
    public void LinkOtherController(InventoryController other) => m_other = other;

    public InventoryModel Model => m_model;
    public InventoryView View => m_view;

    public List<(Item, int)> GetItems()
    {
       return m_model.GetAll();
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= m_capacity)
        {
            Debug.LogError($"Index {index} is out of bounds for inventory of capacity {m_capacity}");
            return;
        }
        m_model.RemoveAt(index);
    }

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

        //m_view.OnDrop += HandleDrop;
        m_model.OnModelChanged += HandleModelChanged;

        RefreshView();
    }

    public void HandleDrop(Slot src, Slot dst)
    {
        // normal same‑panel swap/combine
        int s = src.Index, d = dst.Index;
        if (s == d || dst.ItemId.Equals(SerializableGuid.Empty))
        {
            m_model.Swap(s, d);
        }
        else
        {
            var sId = m_model.Get(s).Details.Id;
            var dId = m_model.Get(d).Details.Id;
            if (sId == dId &&
                m_model.Get(d).Quantity + m_model.Get(s).Quantity
                    <= m_model.Get(d).Details.maxStack)
            {
                m_model.Combine(s, d);
            }
            else
            {
                m_model.Swap(s, d);
            }
        }
    }

    void HandleModelChanged(IList<Item> items) => RefreshView();

    public void RefreshView()
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
        InventoryRole role = InventoryRole.User;
        Func<ItemDetails, bool> onBuy = _ => true;
        Func<Item, bool> onSell = _ => true;

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

        public Builder WithRole(InventoryRole r)
        {
            role = r;
            return this;
        }

        public Builder WithShopHandler(Func<ItemDetails, bool> buyCallback)
        {
            onBuy = buyCallback;
            return this;
        }

        public Builder WithUserHandler(Func<Item, bool> sellCallback)
        {
            onSell = sellCallback;
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

            var ctrl = new InventoryController(model, view, capacity);

            ctrl.SetRole(role);
            ctrl.RegisterShopHandler(onBuy);
            ctrl.RegisterUserHandler(onSell);

            return ctrl;
        }

        public Builder WithContainer(VisualElement container)
        {
            view.SetInventoryHolder(container);
            return this;
        }

        public Builder WithGhostContainer(VisualElement ghostContainer)
        {
            view.SetGhostContainer(ghostContainer);
            return this;
        }
    }

    #endregion
}
