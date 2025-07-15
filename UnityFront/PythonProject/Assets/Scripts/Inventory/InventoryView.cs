using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryView : StorageView
{
    [SerializeField] string m_panelName = "Inventory";

    private VisualElement m_inventoryHolder;

    public override IEnumerator InitializeView(int size = 20)
    {
        // 1) Decide where to build your slots:
        Slots = new Slot[size];
        m_root = m_inventoryHolder != null
               ? m_inventoryHolder
               : m_document.rootVisualElement;

        // 2) Build slot grid under m_root:
        m_root.Clear();
        if (m_styleSheet != null)
            m_root.styleSheets.Add(m_styleSheet);

        m_container = m_root.CreateChild("container");
        var header = m_container.CreateChild("inventoryHeader");
        header.Add(new Label(m_panelName));

        var slotsContainer = header.CreateChild("slotsContainer");
        for (int i = 0; i < size; i++)
        {
            var slot = slotsContainer.CreateChild<Slot>("slot");
            Slots[i] = slot;
        }

        if (StorageView.m_ghostIcon == null)
        {
            var parent = m_ghostContainer ?? m_document.rootVisualElement.Q<VisualElement>("Shop");

            StorageView.m_ghostIcon = new VisualElement();
            StorageView.m_ghostIcon.name = "ghostIcon";
            StorageView.m_ghostIcon.AddToClassList("ghostIcon");

            if (m_styleSheet != null)
                StorageView.m_ghostIcon.styleSheets.Add(m_styleSheet);

            StorageView.m_ghostIcon.style.visibility = Visibility.Hidden;
            StorageView.m_ghostIcon.BringToFront();
            parent.Add(StorageView.m_ghostIcon);
        }

        // 4) Wire up the drag/drop callbacks
        InitView();
        yield return null;
    }

    public void SetInventoryHolder(VisualElement holder)
    {
        m_inventoryHolder = holder;
    }

}
