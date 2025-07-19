using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class StorageView : MonoBehaviour
{
    public Slot[] Slots;

    [SerializeField] protected UIDocument m_document;
    [SerializeField] protected StyleSheet m_styleSheet;

    protected static VisualElement m_ghostIcon;
    protected VisualElement m_ghostContainer;
    static bool m_isDragging;
    static Slot m_originalSlot;

    static Tooltip m_tooltip;
    static Slot m_hoveredSlot;

    protected VisualElement m_root;
    protected VisualElement m_container;

    public void SetGhostContainer(VisualElement gc) => m_ghostContainer = gc;
    public static event Action<Slot, Vector2> OnGlobalDrop;

    public void InitView()
    {
        if (m_tooltip == null) m_tooltip = m_root.parent.CreateChild<Tooltip>("Tooltip");

        m_ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove, TrickleDown.TrickleDown);
        m_ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);

        foreach (var slot in Slots)
        {
            slot.OnStartDrag += OnPointerDown;
            slot.OnHover += OnPointerOver;
            slot.RegisterCallback<PointerLeaveEvent>(e => OnPointerOutSlot(slot, e)); // Pass the slot to the handler
        }
            
    }

    public abstract IEnumerator InitializeView(int size = 20);

    static void OnPointerDown(Vector2 screenPos, Slot slot, PointerDownEvent evt)
    {
        m_isDragging = true;
        m_originalSlot = slot;

        // Configure the ghost icon
        m_ghostIcon.style.backgroundImage = slot.BaseSprite.texture;
        slot.Icon.image = null;
        slot.StackLabel.visible = false;

        m_ghostIcon.CapturePointer(evt.pointerId);

        // Show and position the ghost
        m_ghostIcon.style.opacity = 0.8f;
        m_ghostIcon.style.visibility = Visibility.Visible;
        m_ghostIcon.BringToFront();
        SetGhostIconPosition(screenPos);
    }

    static void OnPointerMove(PointerMoveEvent e)
    {
        if (!m_isDragging) return;
        SetGhostIconPosition(e.position);
    }

    static void OnPointerUp(PointerUpEvent e)
    {
        if (!m_isDragging || m_originalSlot == null)
            return;

        // Broadcast the drop event to whatever is listening
        OnGlobalDrop?.Invoke(m_originalSlot, e.position);

        // Restore the original icon
        if(m_originalSlot.BaseSprite) m_originalSlot.Icon.image = m_originalSlot.BaseSprite.texture;

        // Release pointer capture
        m_originalSlot.ReleasePointer(e.pointerId);
        m_ghostIcon.ReleasePointer(e.pointerId);

        m_ghostIcon.style.visibility = Visibility.Hidden;
        m_ghostIcon.style.backgroundImage = null;

        m_isDragging = false;
        m_originalSlot = null;
    }

    static void OnPointerOver(Slot slot, PointerOverEvent e)
    {
        if (m_hoveredSlot != slot || m_tooltip.style.visibility == Visibility.Hidden)
        {
            m_hoveredSlot = slot;
        }

        if (m_hoveredSlot.ItemId == SerializableGuid.Empty) return;

        ItemDetails item = ItemDatabase.Instance.GetItemDetailsById(m_hoveredSlot.ItemId);
        m_tooltip.Set(item.Name, item.Description, item.OnUse, item.Price);
        m_tooltip.Show();
        m_tooltip.SetPosition(e.position);
    }

    static void OnPointerOutSlot(Slot slot, PointerLeaveEvent e)
    {
        if (!m_tooltip.worldBound.Contains(e.position))
        {
            m_tooltip.Hide();
            m_hoveredSlot = null;
        }
    }

    static void SetGhostIconPosition(Vector2 screenPos)
    {
        // center the ghost under the pointer
        float halfW = m_ghostIcon.worldBound.width * 0.5f;
        float halfH = m_ghostIcon.worldBound.height * 0.5f;
        m_ghostIcon.style.left = screenPos.x - halfW;
        m_ghostIcon.style.top = screenPos.y - halfH;
    }

    /// <summary>
    /// Allows external code to know where the ghost currently is.
    /// </summary>
    public static Rect GetGhostRect() => m_ghostIcon.worldBound;
}
