using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssignmentTracker : MonoBehaviour
{
    private int nextInstanceId = 1;

    private Dictionary<int,AssignmentState> assignments = new();
    private event Action<AssignmentState> OnAssignmentCompleted;
    public event Action<int,int> OnTaskCompleted;

    public AssignmentState CreateNewAssignment(AssignmentDefinition definition)
    {
        int instanceId = nextInstanceId++;
        AssignmentState newState = new AssignmentState(instanceId, definition);
        assignments.Add(instanceId, newState);
        return newState;
    }

    public bool TryCompleteTask(int id, int taskIndex)
    {
        if (!assignments.TryGetValue(id, out AssignmentState currentAssignment)) return false;
        
        bool taskCompleted =  currentAssignment.TryCompleteTask(taskIndex);
        if (!taskCompleted) return false;

        OnTaskCompleted?.Invoke(id, taskIndex);
        return true;
        // OnAssignmentCompleted?.Invoke(currentAssignment);
        
    }

    public bool IsAssignmentCompleted(int id)
    {
        return assignments.TryGetValue(id, out AssignmentState assignment)
            && assignment.isAssignmentCompleted;
    }

    public bool HasEveryTaskCompletedOneTask()
    {
        return assignments.Values.All(assignment => assignment.hasCompletedOneTask);
    }

    public int GetMaximumAssignmentScore()
    {
        return 100 * assignments.Count;
    }

    public int GetTotalAssignmentScore()
    {
        return assignments.Values.Sum(
            assignment => assignment.Score
        );
    }

    public void ResetRun()
    {
        assignments.Clear();
        nextInstanceId = 1;
    }
}
