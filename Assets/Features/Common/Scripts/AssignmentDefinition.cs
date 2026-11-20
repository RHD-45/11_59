using System;
using UnityEngine;

public class AssignmentDefinition : ScriptableObject
{
    public AssigmentType  type;
    public string       displayName;
    public AssignmentTaskDefinition[] tasks;
    public GameObject   prefab;
}

[Serializable]
public enum AssigmentType
{
    Calculus,
    Chemistry,
    ICWire,
    ZaloUpload
}

[Serializable]
public class AssignmentTaskDefinition
{
    public string taskName;
    public int score;
}
