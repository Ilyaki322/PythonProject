using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEngine;
using static UnityEngine.PlayerLoop.PreUpdate;

public class CombinedInventoryManager : MonoBehaviour
{
    [Header("Views")]
    [SerializeField] InventoryView shopView;
    [SerializeField] InventoryView userView;

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

        // 1) Build both controllers (roles no longer needed inside them)
        shopInvController = new InventoryController.Builder(shopView)
            .WithStartingItems(shopItems)
            .WithCapacity(shopCapacity)
            .WithContainer(shopContainer)
            .WithGhostContainer(ghostContainer)
            .Build();

        userInvController = new InventoryController.Builder(userView)
            .WithStartingItems(Array.Empty<ItemDetails>())
            .WithCapacity(userCapacity)
            .WithContainer(playerContainer)
            .WithGhostContainer(ghostContainer)
            .Build();

        StorageView.OnGlobalDrop += HandleGlobalDrop;
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
            var details = shopInvController.Model.Get(src.Index).Details;
            if (!m_shopController.makePurchase(details.Price))
            {
                Debug.Log("Not enough money to buy item: " + details.name);
                return;
            }

            var clone = details.Create(1);
            userInvController.Model.TryAddAt(dst.Index, clone);
            userInvController.RefreshView();
            shopInvController.RefreshView();
            return;
        }

        if (fromUser && toShop)
        {
            var item = userInvController.Model.Get(src.Index);
            if(!m_shopController.sellItem(item.Details.Price))
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
            //shopController.HandleDrop(src, src);
            shopInvController.RefreshView();
            return;
        }

        if (fromUser && toUser)
        {
            userInvController.HandleDrop(src, dst);
            userInvController.RefreshView();
            return;
        }


        shopInvController.RefreshView();
        userInvController.RefreshView();
    }
}

