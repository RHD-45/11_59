using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public AssignmentTracker tracker;
    [SerializeField] public AssignmentSpawner spawner;
    [SerializeField] public ScoreManager scoreManager;
    [SerializeField] public InterruptionManager interruptionManager;
    [SerializeField] public Timer timer;
    [SerializeField] public GameObject BTL;
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;
    public static GameState State {get; private set;} = GameState.None;
    public bool CanPlayerInteract => State == GameState.Playing;
    public bool IsTimerRunning => timer != null && timer.isRunning;


    // debug + end interrupted ở dưới
    [ContextMenu("Starting")]    void DebugStarting()    => ChangeState(GameState.Starting);
    [ContextMenu("Interrupted")] void DebugInterrupted() => ChangeState(GameState.Interrupted);
    [ContextMenu("Submitting")]  void DebugSubmitting()  => ChangeState(GameState.Submitting);
    [ContextMenu("Ended")]       void DebugEnded()       => ChangeState(GameState.Ended);

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        timer.OnTimeExpired += HandleTimeExpired;
        interruptionManager.OnInterruptionFinished += HandleInterruptionFinished;
    } 

    private void Start()
    {
        // Debug.Log("Start() complete");
        ChangeState(GameState.Starting);

    }

    public void ChangeState(GameState newState)
    {
        if (State == newState) return;
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
        - spawn word      -----OK-----
        - bắt đầu spawn btn/noti    ----OK----
        */

        Debug.Log("starting");
        timer.ResetTimer();
        timer.StartTimer();

        tracker.ResetRun();
        spawner.StartRun();

        BTL.SetActive(true);

        ChangeState(GameState.Playing);
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
        scoreManager.FinalizeScore();
    }

    private void HandleTimeExpired() => ChangeState(GameState.Ended);
    private void HandleInterruptionFinished(InterruptionDefinition definition)
    {
        if (State == GameState.Interrupted) EndInterruption(); 
    }

    private void EndRun()
    {
        // to-do
        // - chạy animation done!
        // - tắt timer? ---- OK ----
        timer.StopTimer();
        spawner.StopRun();

        
    }

    private void Result()
    {
        // to-do
        // - chạy result
    }

    private void OnDestroy()
    {
        if (timer != null)
        {
            timer.OnTimeExpired -= HandleTimeExpired;
        }
    }
}

[Serializable]
public enum GameState
{
    None = -1,
    Starting = 0,
    Playing = 1,
    Paused = 2,
    Interrupted = 3,
    Submitting = 4,
    Ended = 5,
    Result = 6
}
