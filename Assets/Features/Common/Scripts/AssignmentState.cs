using System.Linq;

public class AssignmentState
{
    public int InstanceId {get;}
    public AssignmentDefinition Definition {get;}
    private bool[] completedTasks;
    public int taskCount => completedTasks.Length;
    public int completedTaskCount => completedTasks.Count(task => task);
    public bool isComplete => completedTaskCount == taskCount;
    public bool hasCompletedOneTask => completedTaskCount >= 1;

    public AssignmentState (AssignmentDefinition definition)
    {
        Definition = definition;
        completedTasks = new bool[definition.tasks.Length];
    }

    public void CompleteTask(int taskNum)
    {
        // fallback??
        // if (taskNum < 0 || taskNum >= taskCount) return false;
        // if (completedTasks[taskNum]) return false;

        completedTasks[taskNum] = true;
    }
}
