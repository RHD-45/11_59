using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssignmentTracker : MonoBehaviour
{
    private int nextInstanceId = 1;

    private Dictionary<int,AssignmentState> assignments = new();

    public void CreateNewAssignment(AssignmentDefinition definition)
    {
        int instanceId = nextInstanceId++;
        assignments.Add(instanceId, new AssignmentState(definition));
    }

    
}
