using UnityEngine;
using static UnityEditor.Rendering.ShadowCascadeGUI;

public class ShopSocketService : MonoBehaviour
{
    [SerializeField] private SocketManager m_socketManager;
    [SerializeField] private ShopController m_shopController;

    private void Awake()
    {
        m_shopController.OnPurchase += HandlePurchase;
        m_shopController.OnSale += HandleSale;
        m_shopController.OnLevelUp += HandleLevelUp;
    }
    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        m_shopController.OnPurchase -= HandlePurchase;
        m_shopController.OnSale -= HandleSale;
        m_shopController.OnLevelUp -= HandleLevelUp;
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
            currentGold = dto.money
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
            currentGold = dto.money
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
            currentGold = dto.money
        });
    }

}
