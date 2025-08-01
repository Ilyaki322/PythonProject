using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Slot : VisualElement
{
    public Image Icon;
    public Label StackLabel;
    public int Index => parent.IndexOf(this);
    public SerializableGuid ItemId { get; private set; } = SerializableGuid.Empty;
    public Sprite BaseSprite;

    public event Action<Vector2, Slot, PointerDownEvent> OnStartDrag = delegate { };
    public event Action<int, SerializableGuid> OnClick = delegate { };
    public event Action<Slot, PointerOverEvent> OnHover = delegate { };

    public Slot()
    {
        Icon = this.CreateChild<Image>("slotIcon");
        StackLabel = this.CreateChild("slotFrame").CreateChild<Label>("stackCount");
        RegisterCallback<PointerDownEvent>(OnPointerDown);
        RegisterCallback<PointerOverEvent>(OnPointerOver);
    }

    void OnPointerDown(PointerDownEvent e)
    {
        if (e.button != 0 || ItemId.Equals(SerializableGuid.Empty))
        {
            OnClick.Invoke(Index, ItemId);
            return;
        }

        OnClick.Invoke(Index, ItemId);
        OnStartDrag.Invoke(e.position, this, e);
        e.StopPropagation();
    }

    void OnPointerOver(PointerOverEvent e)
    {
        OnHover.Invoke(this, e);
        e.StopPropagation();
    }

    public void Set(SerializableGuid id, Sprite icon, int qty = 0)
    {
        ItemId = id;
        BaseSprite = icon;

        Icon.image = BaseSprite != null ? icon.texture : null;

        StackLabel.text = qty > 1 ? qty.ToString() : string.Empty;
        StackLabel.visible = qty > 1;
    }

    public void Remove()
    {
        ItemId = SerializableGuid.Empty;
        Icon.image = null;
    }
}