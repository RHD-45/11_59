using UnityEngine;

public class AssignmentTracker : MonoBehaviour
{
    public float BTLProgress;
    public float savedBTLProgress;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetAll()
    {
        BTLProgress = 0.0f;
        savedBTLProgress = 0.0f;
    }

    public void AddBTLProgress(float progress)
    {
        BTLProgress += progress;
    }

    public void SaveBTLProgress()
    {
        savedBTLProgress = BTLProgress;
    }

    public void RevertBTLToLastSave()
    {
        BTLProgress = savedBTLProgress;
    }
}
