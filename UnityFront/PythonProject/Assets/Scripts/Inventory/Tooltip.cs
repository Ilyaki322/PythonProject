using System;
using UnityEngine.UIElements;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Tooltip : VisualElement
{
    private Label m_name;
    private Label m_description;
    private Label m_onUse;
    private Label m_price;

    public Tooltip()
    {
        style.visibility = Visibility.Hidden;
        m_name = this.CreateChild<Label>("Tooltip-name");
        m_description = this.CreateChild<Label>("Tooltip-desc");
        m_onUse = this.CreateChild<Label>("Tooltip-use");
        m_price = this.CreateChild<Label>("Tooltip-price");
        pickingMode = PickingMode.Ignore;
    }

    public void Set(string name, string desc, string use, int price)
    {
        m_name.text = name;

        if (string.IsNullOrEmpty(desc)) m_description.text = "";
        else { m_description.text = desc; }

        if (string.IsNullOrEmpty(use)) m_onUse.text = "";
        else{m_onUse.text = "On Use: " + use;}

        if (price < 0) m_price.text = "";
        else{ m_price.text = price.ToString();}  
    }

    public void SetPosition(Vector2 screenPos)
    {
        Vector2 local = parent.WorldToLocal(screenPos);
        style.left = local.x;
        style.top = local.y;
    }

    public void Show()
    {
        this.style.visibility = Visibility.Visible;
    }

    public void Hide()
    {
        this.style.visibility = Visibility.Hidden;
    }
}