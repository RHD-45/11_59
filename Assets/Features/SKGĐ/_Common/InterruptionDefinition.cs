using UnityEngine;

[CreateAssetMenu(fileName = "NewInterruption", menuName = "Game Data/Interruption")]
public class InterruptionDefinition : ScriptableObject
{
    public string displayName;
    public GameObject prefab;
    public float duration = 3f;
}
