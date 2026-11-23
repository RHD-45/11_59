using UnityEngine;
using UnityEngine.UI;

public class Demo : AssignmentWindow
{
    [SerializeField] private Button[] taskButtons;

    // Update is called once per frame
    public override void Initialize(int newAssignmentId, AssignmentTracker newTracker)
    {
        assignmentId = newAssignmentId;
        tracker = newTracker;
        windowFrame.SetCloseStatus(false);

        Debug.Log(assignmentId);
    }

    public void CompleteTask(int taskIndex)
    {
        if (!tracker.TryCompleteTask(assignmentId, taskIndex))
            return;

        taskButtons[taskIndex].interactable = false;

        if (tracker.IsAssignmentCompleted(assignmentId))
        {
            windowFrame.SetCloseStatus(true);
        }
    }
}
