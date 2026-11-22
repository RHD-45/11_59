using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    [SerializeField] private Button[] taskButtons;
    private WindowFrame windowFrame;
    private int assignmentId;
    private AssignmentTracker tracker;

    void Awake()
    {
        windowFrame = GetComponentInParent<WindowFrame>();

        windowFrame.SetCloseStatus(false);
    }

    // Update is called once per frame
    public void Initialize(int newAssignmentId, AssignmentTracker newTracker)
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
