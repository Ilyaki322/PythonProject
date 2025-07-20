using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEngine;

public class CombinedInventoryManager : MonoBehaviour
{
    [Header("Views")]
    [SerializeField] private InventoryView shopView;
    [SerializeField] private InventoryView userView;
    [SerializeField] private InventoryApi userApi;


    [Header("Controller")]
    [SerializeField] ShopController m_shopController;

    VisualElement shopContainer, playerContainer, ghostContainer;

    [Header("Data")]
    [SerializeField] List<ItemDetails> shopItems;
    [SerializeField] int shopCapacity = 10;
    [SerializeField] int userCapacity = 10;

    InventoryController shopInvController, userInvController;

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        shopContainer = root.Q<VisualElement>("ShopInventory");
        playerContainer = root.Q<VisualElement>("UserInventory");
        ghostContainer = root;

        StorageView.OnGlobalDrop += HandleGlobalDrop;
    }

    public List<(Item, int)> userItems()
    {
        return userInvController.GetItems();
    }

    public void RemoveItem(int index)
    {
        userInvController.Model.RemoveAt(index);
    }

    public void InitInventory()
    {
        shopInvController = new InventoryController.Builder(shopView)
            .WithStartingItems(shopItems)
            .WithCapacity(shopCapacity)
            .WithContainer(shopContainer)
            .WithGhostContainer(ghostContainer)
            .Build();

        userInvController = new InventoryController.Builder(userView)
            .WithCapacity(userCapacity)
            .WithContainer(playerContainer)
            .WithGhostContainer(ghostContainer)
            .WithApi(userApi)
            .Build();
    }

    void HandleGlobalDrop(Slot src, Vector2 pointerPos)
    {
        var shopSlots = shopView.Slots;
        var userSlots = userView.Slots;
        var allSlots = shopSlots.Concat(userSlots);

        var ghostRect = StorageView.GetGhostRect();
        var candidates = allSlots
            .Where(s => s.worldBound.Overlaps(ghostRect));

        var dst = candidates
            .OrderBy(s => Vector2.Distance(s.worldBound.center, ghostRect.center))
            .FirstOrDefault();

        bool fromShop = shopSlots.Contains(src);
        bool fromUser = userSlots.Contains(src);
        bool toShop = dst != null && shopSlots.Contains(dst);
        bool toUser = dst != null && userSlots.Contains(dst);

        if (fromShop && toUser)
        {
            var item = shopInvController.Model.Get(src.Index);
            var userSlot = userInvController.Model.Get(dst.Index);

            if (userSlot != null) return;

            var clone = item.Details.Create(1);
            //if (!userInvController.Model.TryAddAt(dst.Index, clone) 
            //    || !m_shopController.makePurchase(item, dst.Index))
            //{
            //    Debug.Log("Not enough money to buy item: " + item.Details.name);
            //    return;
            //}
            if (!(m_shopController.makePurchase(item, dst.Index) && userInvController.Model.TryAddAt(dst.Index, clone)))
            {
                Debug.Log("Not enough money to buy item: " + item.Details.name);
                return;
            }

            userInvController.RefreshView();
            shopInvController.RefreshView();
            return;
        }

        if (fromUser && toShop)
        {
            var item = userInvController.Model.Get(src.Index);
            if(!m_shopController.sellItem(item, src.Index))
            {
                Debug.Log("Not enough money to sell item: " + item.Details.name);
                return;
            }

            userInvController.Model.Remove(item);
            userInvController.RefreshView();
            shopInvController.RefreshView();
            return;
        }

        if (fromShop && toShop)
        {
            shopInvController.RefreshView();
            return;
        }

        if (fromUser && toUser)
        {
            m_shopController.SwapItems(src.Index, dst.Index);
            userInvController.HandleDrop(src, dst);
            userInvController.RefreshView();
            return;
        }


        shopInvController.RefreshView();
        userInvController.RefreshView();
    }
}

