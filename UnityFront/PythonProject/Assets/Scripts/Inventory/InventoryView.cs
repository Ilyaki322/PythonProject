using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryView : StorageView
{
    [SerializeField] string m_panelName = "Inventory";

    public override IEnumerator InitializeView(int size = 20)
    {
        Slots = new Slot[size];
        m_root = m_document.rootVisualElement;
        m_root.Clear();

        m_root.styleSheets.Add(m_styleSheet);
        m_container = m_root.CreateChild("container");

        var inventory = m_container.CreateChild("inventory").WithManipulator(new PanelDragManipulator());
        inventory.CreateChild("inventoryFrame");
        inventory.CreateChild("inventoryHeader").Add(new Label(m_panelName));

        var slotsContainer = inventory.CreateChild("slotsContainer");
        for (int i = 0; i < size; i++)
        {
            var slot = slotsContainer.CreateChild<Slot>("slot");
            Slots[i] = slot;
        }
        m_ghostIcon = m_container.CreateChild("ghostIcon");
        m_ghostIcon.BringToFront();

        yield return null;
    }
}
