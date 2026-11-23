using UnityEngine;

public class ICAssignmentWindow : AssignmentWindow
{
    [SerializeField] private WireDraw[] wires;

    public override void Initialize(
        int newAssignmentId,
        AssignmentTracker newTracker)
    {
        base.Initialize(newAssignmentId, newTracker);

        foreach (WireDraw wire in wires)
        {
            wire.OnWireConnected += HandleWireConnected;
        }
    }

    private void HandleWireConnected(int taskIndex)
    {
        Debug.Log("wire connected");
        if (!tracker.TryCompleteTask(assignmentId, taskIndex))
            return;

        if (tracker.IsAssignmentCompleted(assignmentId))
        {
            windowFrame.SetCloseStatus(true);
        }
        Debug.Log("pass complete");
    }

    private void OnDestroy()
    {
        foreach (WireDraw wire in wires)
        {
            wire.OnWireConnected -= HandleWireConnected;
        }
    }
}