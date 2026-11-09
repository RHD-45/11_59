using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDrag : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private WindowFrame windowFrame;
    private Canvas canvas;
    private Vector2 pointerOffset;

    private void Awake()
    {
        windowFrame = GetComponentInParent<WindowFrame>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData pointerData)
    {


        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            windowFrame.Manager.WindowLayer,
            pointerData.position,
            GetEventCamera(),
            out Vector2 mousePosition
        );

        pointerOffset = windowFrame.Rect.anchoredPosition - mousePosition;

        windowFrame.Manager.ActivateWindow(windowFrame.Rect);
    }

    public void OnDrag(PointerEventData pointerData)
    {

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            windowFrame.Manager.WindowLayer,
            pointerData.position,
            GetEventCamera(),
            out Vector2 mousePosition
        );

        windowFrame.Rect.anchoredPosition = mousePosition + pointerOffset;

    }

    private Camera GetEventCamera()
    {
        return canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;
    }
}