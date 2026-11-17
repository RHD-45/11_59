using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public Timer timer;
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;
    public GameState State {get; private set;}
    public bool CanPlayerInteract => State == GameState.Playing;
    public bool IsTimerRunning => State != GameState.Ended;

    private void Awake()
    {
        timer.OnTimeExpired += EndRun;
    } 

    private void Start()
    {
        ChangeState(GameState.Starting);
        Debug.Log("Start() complete");
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
        - reset btl, saved, điểm, assignment
        - start timer     -----OK-----
        - spawn word
        - bắt đầu spawn btn/noti
        */

        Debug.Log("starting");
        timer.ResetTimer();
        timer.StartTimer();
        

        ChangeState(GameState.Playing);

    }
    // private void HandlePause()
    // {
    //     timer.StopTimer();
    // }

    [ContextMenu("Interrupt")]
    private void BeginInterruption()
    {
        // to-do
    }

    [ContextMenu("End interrupt")]
    private void EndInterruption()
    {
        // to-do
        ChangeState(GameState.Playing);
    }

    [ContextMenu("Submit")]
    private void BeginSubmission()
    {
        // to-do
        
    }

    [ContextMenu("End Game")]
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
