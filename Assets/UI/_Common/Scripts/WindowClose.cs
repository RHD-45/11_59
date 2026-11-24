using UnityEngine;
using UnityEngine.EventSystems;

public class WindowClose : MonoBehaviour, IPointerClickHandler
{
    public WindowFrame windowFrame { get; private set; }

    private void Awake()
    {
        windowFrame = GetComponentInParent<WindowFrame>();

        if (windowFrame == null)
        {
            Debug.LogError($"WindowFrame parent does not exist");
        }
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (windowFrame)
        {
            windowFrame.RequestClose();
        }
    }
}
