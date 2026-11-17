using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private Timer timer;

    private TMP_Text timerText;

    private void Awake()
    {
        timerText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        timer.OnTimeChanged += UpdateTimerText;

        UpdateTimerText(timer.remainingTime);
    }

    private void OnDestroy()
    {
        if (timer != null)
        {
            timer.OnTimeChanged -= UpdateTimerText;
        }
    }

    private void UpdateTimerText(float timeRemaining)
    {
        if (timeRemaining > 0) timerText.text = timer.GetFormattedTime();
        else timerText.text = "00:00:00";
    }
}