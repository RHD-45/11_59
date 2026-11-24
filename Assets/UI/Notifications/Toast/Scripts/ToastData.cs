using System;
using UnityEngine;

public enum ToastType
{
    System = 0,
    PowerCutWarning = 1,
    Assignment = 2,
    Zalo = 3,
    ZaloCall = 4
}

[Serializable]
public class ToastData
{
    public ToastType type;

    public string title;

    public string message;

    public float duration;

    public bool closable;


    public ToastData(
        ToastType toastType,
        string toastTitle,
        string toastMessage,
        float toastDuration = 5f,
        bool allowClose = true)
    {
        type = toastType;
        title = toastTitle;
        message = toastMessage;
        duration = toastDuration;
        closable = allowClose;
    }
}
