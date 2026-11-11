using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class WindowFrame : MonoBehaviour, IPointerDownHandler
{
    public RectTransform Rect { get; private set; }
    public WindowManager Manager { get; private set; }

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
        Manager = GetComponentInParent<WindowManager>();

        if (Manager == null)
        {
            Debug.LogError($"{name}: WindowManager parent does not exist");
        }
    }

    // Giữ lại để dùng khi spawn/pool window sau này.
    public void Initialize(WindowManager manager)
    {
        Manager = manager;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Manager?.ActivateWindow(Rect);
    }

    public void RequestClose()
    {
        if (Manager != null)
        {
            Manager.CloseWindow(gameObject);
        }
    }
}