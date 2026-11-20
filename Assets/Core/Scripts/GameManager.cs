using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;
    public GameState State {get; private set;}
    public bool CanPlayerInteract => State == GameState.Playing;
    public bool IsTimerRunning => State != GameState.Ended;

    void Start() => ChangeState(GameState.Playing);

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
                if (!CanPlayerInteract) EndInterruption();
                break;
            case GameState.Interrupted:
                BeginInterruption();
                break;
            case GameState.Submitting:
                BeginSubmission();
                break;
            case GameState.Ended:
                EndRun();
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
        - start timer
        - spawn word
        - bắt đầu spawn btn/noti
        */

        ChangeState(GameState.Playing);

    }
    private void BeginInterruption()
    {
        // to-do
        ChangeState(GameState.Interrupted);
    }

    private void EndInterruption()
    {
        // to-do
        ChangeState(GameState.Playing);
    }

    private void BeginSubmission()
    {
        // to-do
        ChangeState(GameState.Submitting);
    }

    private void EndRun()
    {
        // to-do
        ChangeState(GameState.Ended);
    }
}

[Serializable]
public enum GameState
{
    Starting = 0,
    Playing = 1,
    Interrupted = 2,
    Submitting = 3,
    Ended = 4
}
