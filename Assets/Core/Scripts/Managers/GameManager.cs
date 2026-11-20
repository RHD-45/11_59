using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public AssignmentTracker tracker;
    [SerializeField] public AssignmentSpawner spawner;
    [SerializeField] public Timer timer;
    [SerializeField] public GameObject BTL;
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;
    public static GameState State {get; private set;}
    public bool CanPlayerInteract => State == GameState.Playing;
    public bool IsTimerRunning => State != GameState.Ended;


    // debug + end interrupted ở dưới
    [ContextMenu("Starting")]    void DebugStarting()    => ChangeState(GameState.Starting);
    [ContextMenu("Interrupted")] void DebugInterrupted() => ChangeState(GameState.Interrupted);
    [ContextMenu("Submitting")]  void DebugSubmitting()  => ChangeState(GameState.Submitting);
    [ContextMenu("Ended")]       void DebugEnded()       => ChangeState(GameState.Ended);

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        timer.OnTimeExpired += EndRun;
    } 

    private void Start()
    {
        Debug.Log("Start() complete");
        DebugStarting();

        ChangeState(GameState.Playing);
    }

    public void ChangeState(GameState newState)
    {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState)
        {
            case GameState.Starting:
                StartRun();
                break;
            case GameState.Playing:
                break;
            // case GameState.Paused:
            //     HandlePause();
            //     break;
            case GameState.Interrupted:
                BeginInterruption();
                break;
            case GameState.Submitting:
                BeginSubmission();
                break;
            case GameState.Ended:
                EndRun();
                break;
            case GameState.Result:
                Result();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);
        Debug.Log($"New state: {newState}");
    }

    private void StartRun()
    {
        /* to-do:
        - reset btl, saved, điểm, assignment    ----2/4---
        - start timer     -----OK-----
        - spawn word
        - bắt đầu spawn btn/noti
        */

        Debug.Log("starting");
        timer.ResetTimer();
        timer.StartTimer();

        BTL.SetActive(true);
    }

    // private void HandlePause()
    // {
    //     timer.StopTimer();
    // }

    private void BeginInterruption()
    {
        // to-do
    }

    [ContextMenu("End Interrupted")]
    private void EndInterruption()
    {
        // to-do
        ChangeState(GameState.Playing);
    }

    private void BeginSubmission()
    {
        // to-do
        
    }

    private void EndRun()
    {
        // to-do
        // - chạy animation done!
        // - tắt timer? ---- OK ----
        timer.StopTimer();
    }

    private void Result()
    {
        // to-do
        // - chạy result
    }
}

[Serializable]
public enum GameState
{
    Starting = 0,
    Playing = 1,
    Paused = 2,
    Interrupted = 3,
    Submitting = 4,
    Ended = 5,
    Result = 6
}
