using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : Singleton<NotificationManager>
{
    [Header("Toast Manager Settings")]
    [SerializeField] private RectTransform toastContainer;
    [SerializeField] private SystemToastPool systemToastPool;
    [SerializeField] private int maxVisibleToasts = 12;

    private Queue<ToastData> pendingQueue = new Queue<ToastData>();
    private List<ToastNotificationBase> activeToasts = new List<ToastNotificationBase>();

    private void OnEnable()
    {
        EventBroker.onToastRequested += HandleToastRequested;
        EventBroker.onClearAllToasts += ClearAllToasts;
        GameManager.OnAfterStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        EventBroker.onToastRequested -= HandleToastRequested;
        EventBroker.onClearAllToasts -= ClearAllToasts;
        GameManager.OnAfterStateChanged -= HandleGameStateChanged;
    }

    public void EnqueueToast(ToastData data)
    {
        if (data == null)
        {
            return;
        }

        if (activeToasts.Count < maxVisibleToasts)
        {
            DisplayToast(data);
        }
        else
        {
            pendingQueue.Enqueue(data);
        }
    }

    public void ClearAllToasts()
    {
        pendingQueue.Clear();

        for (int i = activeToasts.Count - 1; i >= 0; i--)
        {
            ToastNotificationBase toast = activeToasts[i];
            if (toast != null)
            {
                toast.ForceCloseImmediate();
            }
        }
    }

    // Event handlers
    private void HandleToastRequested(ToastData data)
    {
        EnqueueToast(data);
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.Interrupted || newState == GameState.Ended)
        {
            ClearAllToasts();
        }
    }

    // Display & pool management
    private void DisplayToast(ToastData data)
    {
        if (systemToastPool == null || toastContainer == null)
        {
            return;
        }

        SystemToast toast = systemToastPool.GetObject();
        if (toast == null)
        {
            return;
        }

        toast.transform.SetParent(toastContainer, false);
        toast.transform.SetAsLastSibling();

        toast.OnToastClosed += HandleToastClosed;
        activeToasts.Add(toast);
        toast.Show(data);
    }

    private void HandleToastClosed(ToastNotificationBase toast)
    {
        if (toast == null)
        {
            return;
        }

        toast.OnToastClosed -= HandleToastClosed;
        activeToasts.Remove(toast);

        SystemToast systemToast = toast as SystemToast;
        if (systemToast != null && systemToastPool != null)
        {
            systemToastPool.ReturnObject(systemToast);
        }

        TryShowNextQueuedToast();
    }

    private void TryShowNextQueuedToast()
    {
        while (activeToasts.Count < maxVisibleToasts && pendingQueue.Count > 0)
        {
            ToastData nextData = pendingQueue.Dequeue();
            DisplayToast(nextData);
        }
    }
}
