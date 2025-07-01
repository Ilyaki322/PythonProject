using UnityEngine;
using UnityEngine.UIElements;

public class PanelDragManipulator : PointerManipulator
{
    bool m_isDragging;
    Vector2 m_offset;

    public PanelDragManipulator()
    {
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
    }
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        if (!CanStartManipulation(evt) || m_isDragging) return;

        m_offset = evt.localPosition;
        m_isDragging = true;

        target.CapturePointer(evt.pointerId);
        evt.StopPropagation();
    }

    void OnPointerMove(PointerMoveEvent evt)
    {
        if (!m_isDragging || !target.HasPointerCapture(evt.pointerId)) return;

        Vector3 delta = evt.localPosition - (Vector3)m_offset;
        target.transform.position += delta;
        evt.StopPropagation();
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (!CanStopManipulation(evt) || !m_isDragging) return;

        m_isDragging = false;
        target.ReleasePointer(evt.pointerId);
        evt.StopPropagation();
    }
}
