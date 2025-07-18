using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryView : StorageView
{
    [SerializeField] string m_panelName = "Inventory";

    private VisualElement m_inventoryHolder;

    public override IEnumerator InitializeView(int size = 20)
    {
        Slots = new Slot[size];
        m_root = m_inventoryHolder != null
               ? m_inventoryHolder
               : m_document.rootVisualElement;

        m_root.Clear();
        if (m_styleSheet != null)
            m_root.styleSheets.Add(m_styleSheet);

        //m_container = m_root.CreateChild("container");
        m_root.AddToClassList("inventory");
        var header = m_root.CreateChild("inventoryHeader");
        var label = new Label(m_panelName);
        label.AddToClassList("labelProps");
        header.Add(label);


        var slotsContainer = header.CreateChild("slotsContainer");
        for (int i = 0; i < size; i++)
        {
            var slot = slotsContainer.CreateChild<Slot>("slot");
            Slots[i] = slot;
        }

        if (StorageView.m_ghostIcon == null)
        {
            var parent = m_ghostContainer;

            StorageView.m_ghostIcon = new VisualElement();
            StorageView.m_ghostIcon.name = "ghostIcon";
            StorageView.m_ghostIcon.AddToClassList("ghostIcon");

            if (m_styleSheet != null)
                StorageView.m_ghostIcon.styleSheets.Add(m_styleSheet);

            StorageView.m_ghostIcon.style.visibility = Visibility.Hidden;
            StorageView.m_ghostIcon.BringToFront();
            parent.Add(StorageView.m_ghostIcon);
        }

        InitView();
        yield return null;
    }

    public void SetInventoryHolder(VisualElement holder)
    {
        m_inventoryHolder = holder;
    }

}
