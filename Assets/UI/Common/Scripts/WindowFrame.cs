using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class WindowFrame : MonoBehaviour, IPointerDownHandler
{
    public RectTransform Rect { get; private set; }
    public WindowManager Manager { get; private set; }

    private bool canClose = true;

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
        Manager = GetComponentInParent<WindowManager>();    // gắn vào WindowLayer

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

    public bool SetCloseStatus(bool status) => canClose = status;

    public void RequestClose()
    {
        if (!canClose)
        {
            Debug.Log("BTN chưa xong");
            return;
        }

        if (Manager != null)
        {
            Manager.CloseWindow(gameObject);
        }
    }

    public bool IsFocused
{
    get
    {
        return Manager != null &&
               Manager.ActiveWindow == Rect;
    }
}
}