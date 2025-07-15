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

    VisualElement shopContainer, playerContainer, ghostContainer;

    [Header("Data")]
    [SerializeField] List<ItemDetails> shopItems;
    [SerializeField] int shopCapacity = 10;
    [SerializeField] int userCapacity = 10;

    InventoryController shopController, userController;

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        shopContainer = root.Q<VisualElement>("ShopInventory");
        playerContainer = root.Q<VisualElement>("UserInventory");
        ghostContainer = root;

        // 1) Build both controllers (roles no longer needed inside them)
        shopController = new InventoryController.Builder(shopView)
            .WithStartingItems(shopItems)
            .WithCapacity(shopCapacity)
            .WithContainer(shopContainer)
            .WithGhostContainer(ghostContainer)
            .Build();

        userController = new InventoryController.Builder(userView)
            .WithStartingItems(Array.Empty<ItemDetails>())
            .WithCapacity(userCapacity)
            .WithContainer(playerContainer)
            .WithGhostContainer(ghostContainer)
            .Build();

        StorageView.OnGlobalDrop += HandleGlobalDrop;
    }

    void HandleGlobalDrop(Slot src, Vector2 pointerPos)
    {
        // gather all slots:
        var shopSlots = shopView.Slots;
        var userSlots = userView.Slots;
        var allSlots = shopSlots.Concat(userSlots);

        // find the slot whose worldBound overlaps the ghost:
        var ghostRect = StorageView.GetGhostRect();
        var candidates = allSlots
            .Where(s => s.worldBound.Overlaps(ghostRect));

        var dst = candidates
            .OrderBy(s => Vector2.Distance(s.worldBound.center, ghostRect.center))
            .FirstOrDefault();

        // now decide which case it is:
        bool fromShop = shopSlots.Contains(src);
        bool fromUser = userSlots.Contains(src);
        bool toShop = dst != null && shopSlots.Contains(dst);
        bool toUser = dst != null && userSlots.Contains(dst);

        // 1) BUY: shop→user
        if (fromShop && toUser)
        {
            var details = shopController.Model.Get(src.Index).Details;
            // ... your buy check ...
            var clone = details.Create(1);
            userController.Model.TryAddAt(dst.Index, clone);
            userController.RefreshView();
            shopController.RefreshView();
            return;
        }

        // 2) SELL: user→shop
        if (fromUser && toShop)
        {
            var item = userController.Model.Get(src.Index);
            // ... your sell check ...
            userController.Model.Remove(item);
            userController.RefreshView();
            shopController.RefreshView();
            return;
        }

        // 3) intra‑shop
        if (fromShop && toShop)
        {
            //shopController.HandleDrop(src, src);
            shopController.RefreshView();
            return;
        }

        // 4) intra‑user
        if (fromUser && toUser)
        {
            userController.HandleDrop(src, dst);
            userController.RefreshView();
            return;
        }

        // 5) anything else → snap both back
        shopController.RefreshView();
        userController.RefreshView();
    }
}

