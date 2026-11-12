using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{
    [SerializeField] private Slider stressSlider;

    [Header("Stress Bar Settings")]
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private float minInterval = 0.3f;

    private float _lastClickTime = -999f;
    private float burstStress = 0;

    private float _targetValue = 0;

    public void SetStress(float normalizedValue)
    {
        float currentTime = Time.time;
        float delta = currentTime - _lastClickTime;

        if(delta < minInterval) burstStress +=0.0001f;
        else burstStress =0 ;

        _targetValue += Mathf.Clamp01(normalizedValue + burstStress);
        _lastClickTime = currentTime;
    }

    private void Update()
    {
        stressSlider.value = Mathf.Lerp(stressSlider.value, _targetValue, Time.deltaTime * smoothSpeed);
        _targetValue = Mathf.Max(0f, _targetValue - 0.0005f);
    }
}
