using System.Linq;

public class AssignmentState
{
    public int InstanceId {get;}
    public AssignmentDefinition Definition {get;}
    private bool[] completedTasks;
    public int taskCount => completedTasks.Length;
    public int completedTaskCount => completedTasks.Count(task => task);
    public bool isAssignmentCompleted => completedTaskCount == taskCount;
    public bool hasCompletedOneTask => completedTaskCount >= 1;

    public AssignmentState (int id, AssignmentDefinition definition)
    {
        InstanceId = id;
        Definition = definition;
        completedTasks = new bool[definition.tasks.Length];
    }

    public void CompleteTask(int taskIndex)
    {
        // fallback??
        // if (taskNum < 0 || taskNum >= taskCount) return false;
        // if (completedTasks[taskNum]) return false;

        completedTasks[taskIndex] = true;
    }

    public bool IsTaskComplete(int taskIndex)
    {
        return taskIndex >= 0 && taskIndex < taskCount && completedTasks[taskIndex];
    }
}
