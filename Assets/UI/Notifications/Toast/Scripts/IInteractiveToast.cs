public interface IInteractiveToast
{
    bool IsWaitingForInput { get; }

    // Toggle interactable buttons
    void SetInteractable(bool interactable);
}
