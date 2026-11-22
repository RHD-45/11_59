using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssignmentTracker : MonoBehaviour
{
    private int nextInstanceId = 1;

    private Dictionary<int,AssignmentState> assignments = new();
    private event Action<AssignmentState> OnAssignmentCompleted;

    public void CreateNewAssignment(AssignmentDefinition definition)
    {
        int instanceId = nextInstanceId++;
        assignments.Add(instanceId, new AssignmentState(instanceId, definition));
    }

    public void CompleteTask(int id, int taskIndex)
    {
        AssignmentState currentAssignment = assignments[id];
        currentAssignment.CompleteTask(taskIndex);
        // OnAssignmentCompleted?.Invoke(currentAssignment);
        
    }

    public bool hasEveryTaskCompletedOneTask()
    {
        return assignments.Values.All(assignment => assignment.hasCompletedOneTask);
    }
}
