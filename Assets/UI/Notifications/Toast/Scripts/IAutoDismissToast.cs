// Interface for Auto Dismiss Notifications for a period of time
public interface IAutoDismissToast
{
    float AutoDismissDuration { get; }

    void StartAutoDismiss(float duration);
}
