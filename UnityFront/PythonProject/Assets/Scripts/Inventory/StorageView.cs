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

    void Start()
    {
        m_ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        m_ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);

        foreach (Slot slot in Slots)
        {
            slot.OnStartDrag += OnPointerDown;
        }
    }

    public abstract IEnumerator InitializeView(int size = 20);

    static void OnPointerDown(Vector2 position, Slot slot)
    {
        m_isDragging = true;
        m_originalSlot = slot;

        SetGhostIconPosition(position);
        m_ghostIcon.style.backgroundImage = m_originalSlot.BaseSprite.texture;
        m_originalSlot.Icon.image = null;
        m_originalSlot.StackLabel.visible = false;

        m_ghostIcon.style.opacity = 0.8f;
        m_ghostIcon.style.visibility = Visibility.Visible;
    }

    static void SetGhostIconPosition(Vector2 pos)
    {
        m_ghostIcon.style.top = pos.y - m_ghostIcon.layout.height / 2;
        m_ghostIcon.style.left = pos.x - m_ghostIcon.layout.width / 2;
    }

    void OnPointerMove(PointerMoveEvent e)
    {
        if (!m_isDragging) return;

        SetGhostIconPosition(e.position);
    }

    void OnPointerUp(PointerUpEvent e)
    {
        if (!m_isDragging) return;

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

        m_isDragging = false;
        m_originalSlot = null;
        m_ghostIcon.style.visibility = Visibility.Hidden;
    }
}
