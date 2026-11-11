using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowManager : MonoBehaviour
{
    [SerializeField] private float screenPadding = 12f;

    private readonly List<RaycastResult> raycastResults = new();

    public RectTransform ActiveWindow { get; private set; }

    public static WindowManager Instance { get; private set; }

    public RectTransform WindowLayer { get; private set; }

    private void Awake()
    {
        Instance = this;

        WindowLayer = transform.Find("WindowLayer") as RectTransform;

        if (WindowLayer == null)
        {
            Debug.LogError("No WindowLayer child found");
        }
    }

    private void Update()
    {
        // Click vào đâu thì xét object UI trên cùng tại điểm đó.
        if (!Input.GetMouseButtonDown(0) || EventSystem.current == null)
            return;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        raycastResults.Clear();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        if (raycastResults.Count == 0)
            return;

        // Chỉ xét object UI nằm trên cùng.
        RectTransform clickedWindow = FindWindowRoot(raycastResults[0].gameObject.transform);

        Debug.Log(clickedWindow);
        if (clickedWindow != null)
            ActivateWindow(clickedWindow);
    }

    public void ActivateWindow(RectTransform window)
    {
        if (window == null || window.parent != WindowLayer)
            return;

        window.SetAsLastSibling();
        ActiveWindow = window;

        
    }

    public void ClampToWindowLayer(RectTransform window)
    {
        Vector2 layerSize = WindowLayer.rect.size;
        Vector2 windowSize = window.rect.size;
        Vector2 position = window.anchoredPosition;

        // Code này giả định WindowFrame anchor/pivot đều ở Middle Center.
        float maxX = Mathf.Max(
            0f,
            (layerSize.x - windowSize.x) * 0.5f - screenPadding
        );

        float maxY = Mathf.Max(
            0f,
            (layerSize.y - windowSize.y) * 0.5f - screenPadding
        );

        position.x = Mathf.Clamp(position.x, -maxX, maxX);
        position.y = Mathf.Clamp(position.y, -maxY, maxY);

        window.anchoredPosition = position;
    }

    public void CloseWindow(GameObject targetWindow)
    {
        if (targetWindow == null)
            return;

        if (ActiveWindow != null &&
            ActiveWindow.gameObject == targetWindow)
        {
            ActiveWindow = null;
        }

        targetWindow.SetActive(false);
    }

    public void OpenWindow(GameObject targetWindow)
    {
        if (targetWindow == null)
            return;

        targetWindow.SetActive(true);

        RectTransform window = targetWindow.GetComponent<RectTransform>();
        ActivateWindow(window);
    }

    private RectTransform FindWindowRoot(Transform clickedObject)
    {
        Transform current = clickedObject;

        while (current != null)
        {
            // Window root phải là con trực tiếp của WindowLayer.
            if (current.parent == WindowLayer)
                return current as RectTransform;

            // Click Modal/Desktop/Notification thì không tìm xuống WindowLayer.
            if (current == WindowLayer)
                return null;

            current = current.parent;
        }

        return null;
    }
}