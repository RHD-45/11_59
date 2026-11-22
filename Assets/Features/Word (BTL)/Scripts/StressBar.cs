
using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{
    [SerializeField] private Slider stressSlider;

    [Header("Stress Bar Settings")]
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private float minInterval = 0.3f;
    [SerializeField] private float maxStressThreshold = 0.99f;
    [SerializeField] private float cooldownSpeed = 0.5f;

    private float _lastClickTime = -999f;
    private float burstStress = 0f;
    private float _targetValue = 0f;

    private bool can_type = true;

    public void SetStress(float normalizedValue)
    {
        float currentTime = Time.time;
        float delta = currentTime - _lastClickTime;

        if (delta < minInterval)
            burstStress += 0.0001f;
        else
            burstStress = 0f;

        _targetValue += Mathf.Clamp01(normalizedValue + burstStress);
        _targetValue = Mathf.Clamp01(_targetValue);

        _lastClickTime = currentTime;
    }

    public float GetStress()
    {
        return _targetValue;
    }

    public void SetStressValue(float value)
    {
        _targetValue = Mathf.Clamp01(value);
        stressSlider.value = _targetValue;

        can_type = _targetValue < maxStressThreshold;
        burstStress = 0f;
        _lastClickTime = -999f;
    }

    private void Update()
    {
        _targetValue = Mathf.Max(0f, _targetValue - cooldownSpeed * 0.001f);

        stressSlider.value = Mathf.Lerp(
            stressSlider.value,
            _targetValue,
            Time.deltaTime * smoothSpeed
        );

        if (stressSlider.value >= maxStressThreshold ||
            _targetValue >= maxStressThreshold)
        {
            can_type = false;
        }

        if (_targetValue <= 0f && stressSlider.value <= 0.001f)
        {
            stressSlider.value = 0f;
            can_type = true;
        }
    }

    public bool GetCanType()
    {
        return can_type;
    }
}
