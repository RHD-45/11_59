
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UIRaycastDebug : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        PointerEventData data =
            new PointerEventData(EventSystem.current);

        data.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results =
            new List<RaycastResult>();

        EventSystem.current.RaycastAll(data, results);

        Debug.Log("===== UI RAYCAST RESULTS =====");

        foreach (RaycastResult result in results)
        {
            Debug.Log("HIT: " + result.gameObject.name);
        }
    }
}
