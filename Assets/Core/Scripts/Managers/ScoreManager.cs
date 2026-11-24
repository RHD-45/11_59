using UnityEngine;
using System;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private TypeWriterText BTL;
    [SerializeField] private AssignmentTracker BTN;
    [SerializeField] private AssignmentSpawner taskList;
    [SerializeField] private float maxBTLScore = 850f;
    [SerializeField] private float maxBTNScore = 150f;
    [SerializeField] private int BTLScore;
    [SerializeField] private int BTNScore;
    [SerializeField] private int TotalScore;

    //DEBUG
    private int GetBTLScore()
    {
        float BTLProgress = BTL.GetBTLProgress();
        BTLScore = Mathf.FloorToInt(BTLProgress * maxBTLScore);
        return BTLScore;
    }

    private int GetBTNScore()
    {
        float totalBTNRawScore = BTN.GetTotalAssignmentScore();
        float maxBTNRawScore = BTN.GetMaximumAssignmentScore();

        BTNScore = Mathf.FloorToInt(totalBTNRawScore/maxBTNRawScore * maxBTNScore);
        return BTNScore;
    }

    public void FinalizeScore()
    {
        TotalScore = Mathf.FloorToInt(GetBTLScore() + GetBTNScore());
    }



        // ----- DEBUG -----
    private void OnEnable()
    {
        BTL.OnBTLChanged += HandleBTLChanged;
        BTN.OnTaskCompleted += HandleBTNChanged;
    }

    private void OnDisable()
    {
        BTL.OnBTLChanged -= HandleBTLChanged;
        BTN.OnTaskCompleted -= HandleBTNChanged;
    }

    private void HandleBTLChanged()
    {
        TotalScore = GetBTLScore() + BTNScore;
    }
    private void HandleBTNChanged(int id, int taskIndex)
    {
        TotalScore = BTLScore + GetBTNScore();
        //score display
    }

    // ----- DEBUG -----
}
