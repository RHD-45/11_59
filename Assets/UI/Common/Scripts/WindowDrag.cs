using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class WindowDrag : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform window;
    [SerializeField] private RectTransform windowLayer;

    private Vector2 pointerOffset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            windowLayer,
            eventData.position,
            GetEventCamera(),
            out Vector2 pointerLocalPosition
        );

        // đổi pos tại chỗ thì window sẽ nhảy vào tâm chuột -> tính offset
        pointerOffset = window.anchoredPosition - pointerLocalPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            windowLayer,
            eventData.position,
            GetEventCamera(),
            out Vector2 pointerLocalPosition
        );

        window.anchoredPosition = pointerLocalPosition + pointerOffset;

        // WindowManager.ClampToScreen(window) here ---------------------
    }

    private Camera GetEventCamera()
    {
        return canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;
    }
}