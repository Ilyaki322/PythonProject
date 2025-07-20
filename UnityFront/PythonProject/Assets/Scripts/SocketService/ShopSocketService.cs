using UnityEngine;

public class ShopSocketService : MonoBehaviour
{
    [SerializeField] private SocketManager m_socketManager;
    [SerializeField] private ShopController m_shopController;

    private void Awake()
    {
        m_shopController.OnPurchase += HandlePurchase;
        m_shopController.OnSale += HandleSale;
        m_shopController.OnLevelUp += HandleLevelUp;
        m_shopController.OnSwapItems += SwapItemsCall;
    }
    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        m_shopController.OnPurchase -= HandlePurchase;
        m_shopController.OnSale -= HandleSale;
        m_shopController.OnLevelUp -= HandleLevelUp;
        m_shopController.OnSwapItems -= SwapItemsCall;
    }

    private void HandlePurchase(string TargetitemId, int qty, int slotIndexTarget)
    {
        var dto = m_shopController.SelectedCharacter;
        m_socketManager.Emit("PurchaseItem", new
        {
            charId = dto.id,
            itemId = TargetitemId,
            count = qty,
            slotIndex = slotIndexTarget,
            cost = ItemDatabase.Instance.GetItemDetailsById(TargetitemId).Price,
        });
    }

    private void HandleSale(string TargetitemId, int qty, int slotIndexTarget)
    {
        var dto = m_shopController.SelectedCharacter;
        m_socketManager.Emit("SellItem", new
        {
            charId = dto.id,
            itemId = TargetitemId,
            count = qty,
            slotIndex = slotIndexTarget,
            cost = ItemDatabase.Instance.GetItemDetailsById(TargetitemId).Price,
        });
        Debug.Log($"Emitting SellItem for {TargetitemId}");
    }

    private void HandleLevelUp(int UpdatedLevel)
    {
        var dto = m_shopController.SelectedCharacter;
        m_socketManager.Emit("LevelUp", new
        {
            charId = dto.id,
            newLevel = UpdatedLevel,
            currentGold = dto.CharMoney
        });
    }

    private void SwapItemsCall(int src, int dest)
    {
        m_socketManager.Emit("SwapSlots", new
        {
            charId = m_shopController.SelectedCharacter.id,
            indexSrc = src,
            indexDest = dest
        });
    }
}
