using UnityEngine;

public abstract class AssignmentWindow : MonoBehaviour
{
    protected int assignmentId;
    protected AssignmentTracker tracker;
    protected WindowFrame windowFrame;

    protected virtual void Awake()
    {
        windowFrame = GetComponentInParent<WindowFrame>();
    }

    public virtual void Initialize(int newAssignmentId, AssignmentTracker newTracker)
    {
        assignmentId = newAssignmentId;
        tracker = newTracker;
        windowFrame.SetCloseStatus(false);
    }
}