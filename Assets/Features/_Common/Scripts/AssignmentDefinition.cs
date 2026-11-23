using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBTN", menuName = "Game Data/BTN")]
public class AssignmentDefinition : ScriptableObject
{
    public AssigmentType  type;
    public string       displayName;
    public AssignmentTaskDefinition[] tasks;
    public GameObject   prefab;
    public List<int> rewardForCompletionOrder;
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
