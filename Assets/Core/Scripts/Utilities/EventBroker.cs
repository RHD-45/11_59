using System;
using UnityEngine;

public static class EventBroker
{
    // Notifications 
    public static Action<ToastData> onToastRequested;
    public static Action onClearAllToasts;
}
