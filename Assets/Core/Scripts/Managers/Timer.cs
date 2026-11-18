using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    [SerializeField] private float TimeLimit;

    public float remainingTime {get; private set; }
    public bool isRunning {get; private set; }

    public event Action<float> OnTimeChanged;
    public event Action OnTimeExpired;

    private void Awake()
    {
        ResetTimer();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isRunning) return;

        if (remainingTime > 0) 
        {
            remainingTime -= Time.deltaTime;
            OnTimeChanged?.Invoke(remainingTime);
        }
        else if (remainingTime <= 0)
        {
            remainingTime = 0;
            isRunning = false;
            OnTimeExpired?.Invoke();
        }

    }

    public void ResetTimer()
    {
        remainingTime = TimeLimit;
        isRunning = false;
        OnTimeChanged?.Invoke(remainingTime);
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        return $"23:{(59-minutes):00}:{59-seconds:00}";
    }
}
