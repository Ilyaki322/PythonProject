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

    static bool m_isDragging;
    static Slot m_originalSlot;

    protected VisualElement m_root;
    protected VisualElement m_container;

    public event Action<Slot, Slot> OnDrop;

    public void InitView()
    {
        m_ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        m_ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);

        foreach (Slot slot in Slots)
        {
            slot.OnStartDrag += OnPointerDown;
        }
    }

    public abstract IEnumerator InitializeView(int size = 20);

    static void OnPointerDown(Vector2 position, Slot slot, PointerDownEvent evt)
    {
        m_isDragging = true;
        m_originalSlot = slot;

        SetGhostIconPosition(position);
        m_ghostIcon.style.backgroundImage = m_originalSlot.BaseSprite.texture;
        m_originalSlot.Icon.image = null;
        m_originalSlot.StackLabel.visible = false;

        slot.CapturePointer(evt.pointerId);
        m_ghostIcon.CapturePointer(evt.pointerId);

        m_ghostIcon.style.opacity = 0.8f;
        m_ghostIcon.style.visibility = Visibility.Visible;
        m_ghostIcon.BringToFront();
    }

    static void SetGhostIconPosition(Vector2 globalPos)
    {
        // Convert into the ghost’s parent coordinate space:
        var parent = m_ghostIcon.parent;
        Vector2 local = parent.WorldToLocal(globalPos);

        // Center the ghost under the pointer:
        float halfW = m_ghostIcon.layout.width * 0.5f;
        float halfH = m_ghostIcon.layout.height * 0.5f;

        m_ghostIcon.style.left = local.x - halfW;
        m_ghostIcon.style.top = local.y - halfH;
    }

    void OnPointerMove(PointerMoveEvent e)
    {
        Debug.Log($"Before IF OnPointerUp: {m_originalSlot.ItemId} at {e.position}");
        if (!m_isDragging) return;
        Debug.Log($"OnPointerUp: {m_originalSlot.ItemId} at {e.position}");
        SetGhostIconPosition(e.position);
    }

    void OnPointerUp(PointerUpEvent e)
    {
        if (!m_isDragging) return;
        Debug.Log($"OnPointerUp: {m_originalSlot.ItemId} at {e.position}");
        Slot closestSlot = Slots
            .Where(slot => slot.worldBound.Overlaps(m_ghostIcon.worldBound))
            .OrderBy(slot => Vector2.Distance(slot.worldBound.position, m_ghostIcon.worldBound.position))
            .FirstOrDefault();

        if (closestSlot != null)
        {
            OnDrop?.Invoke(m_originalSlot, closestSlot);
        } 
        else
        {
            m_originalSlot.Icon.image = m_originalSlot.BaseSprite.texture;
        }

        m_originalSlot.ReleasePointer(e.pointerId);
        m_ghostIcon.ReleasePointer(e.pointerId);

        m_isDragging = false;
        m_originalSlot = null;
        m_ghostIcon.style.visibility = Visibility.Hidden;
    }
}
