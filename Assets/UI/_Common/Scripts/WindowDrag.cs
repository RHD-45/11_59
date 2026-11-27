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
        // if (windowFrame == null) Debug.Log("no window frame");
        canvas = GetComponentInParent<Canvas>();
        // if (canvas == null) Debug.Log("no canvas");
    }

    public void OnBeginDrag(PointerEventData pointerData)
    {

        Debug.Log(
            $"Frame: {windowFrame}, " +
            $"Manager: {windowFrame?.Manager}, " +
            $"Layer: {windowFrame?.Manager?.WindowLayer}, " +
            $"Canvas: {canvas}"
        );

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