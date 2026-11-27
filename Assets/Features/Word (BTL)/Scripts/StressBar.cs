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

    [Header("Visual Feedback Settings")]
    [SerializeField] private float shakeTriggerRatio = 0.8f;
    [SerializeField] private float shakeMagnitude = 6f;
    [SerializeField] private float tiltAngle = 5.62f;
    [SerializeField] private float tiltSpeed = 10f;

    private RectTransform _barRect;
    private Vector2 _initialAnchoredPos;
    private Quaternion _initialRotation;

    private float _lastClickTime = -999f;
    private float burstStress = 0f;
    private float _targetValue = 0f;

    private bool can_type = true;

    private void Awake()
    {
        if (stressSlider != null)
        {
            _barRect = stressSlider.GetComponent<RectTransform>();
            _initialAnchoredPos = _barRect.anchoredPosition;
            _initialRotation = _barRect.localRotation;
        }
    }

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

        UpdateBarTransform();
    }

    private void UpdateBarTransform()
    {
        if (_barRect == null) return;

        float shakeStartThreshold = maxStressThreshold * shakeTriggerRatio;

        if (!can_type)
        {
            _barRect.anchoredPosition = Vector2.Lerp(
                _barRect.anchoredPosition,
                _initialAnchoredPos,
                Time.deltaTime * tiltSpeed
            );

            Quaternion targetTilt = _initialRotation * Quaternion.Euler(0f, 0f, tiltAngle);
            _barRect.localRotation = Quaternion.Lerp(
                _barRect.localRotation,
                targetTilt,
                Time.deltaTime * tiltSpeed
            );
        }
        else if (stressSlider.value >= shakeStartThreshold)
        {
            float intensity = Mathf.InverseLerp(shakeStartThreshold, maxStressThreshold, stressSlider.value);
            Vector2 shakeOffset = Random.insideUnitCircle * (shakeMagnitude * Mathf.Lerp(0.4f, 1f, intensity));

            _barRect.anchoredPosition = _initialAnchoredPos + shakeOffset;
            _barRect.localRotation = Quaternion.Lerp(
                _barRect.localRotation,
                _initialRotation,
                Time.deltaTime * tiltSpeed
            );
        }
        else
        {
            _barRect.anchoredPosition = Vector2.Lerp(
                _barRect.anchoredPosition,
                _initialAnchoredPos,
                Time.deltaTime * tiltSpeed
            );
            _barRect.localRotation = Quaternion.Lerp(
                _barRect.localRotation,
                _initialRotation,
                Time.deltaTime * tiltSpeed
            );
        }
    }

    public bool GetCanType()
    {
        return can_type;
    }
}