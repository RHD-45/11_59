
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider progressbar;
    [SerializeField] private float smoothSpeed = 8f;

    private int maxWords;
    private float targetValue;

    public void SetMaxWords(int totalWords)
    {
        maxWords = Mathf.Max(1, totalWords);
        targetValue = 0f;
        progressbar.value = 0f;
    }

    public void UpdateProgress(int totalWordsTyped)
    {
        targetValue = Mathf.Clamp01(
            (float)totalWordsTyped / maxWords
        );
    }

    public float GetProgress()
    {
        return targetValue;
    }

    public void SetProgress(float value)
    {
        targetValue = Mathf.Clamp01(value);
        progressbar.value = targetValue;
    }

    private void Update()
    {
        progressbar.value = Mathf.Lerp(
            progressbar.value,
            targetValue,
            Time.deltaTime * smoothSpeed
        );
    }
}
