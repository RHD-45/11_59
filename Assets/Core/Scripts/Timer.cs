using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
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
        }
        else if (remainingTime < 0)
        {
            remainingTime = 0;
            isRunning = false;
            OnTimeExpired?.Invoke();
            // run endgame
        }

    }

    private void ResetTimer()
    {
        remainingTime = TimeLimit;
        isRunning = false;
        OnTimeChanged?.Invoke(remainingTime);
    }

    private void StartTimer()
    {
        isRunning = true;
    }

    private void StopTimer()
    {
        isRunning = false;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.CeilToInt(remainingTime / 60);
        int seconds = Mathf.CeilToInt(remainingTime % 60);

        return $"23:{minutes:00}:{seconds:00}";
    }
}
