using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class ToastNotificationBase : MonoBehaviour
{
    // Configurations
    [SerializeField] protected RectTransform visualPanel;

    [SerializeField] protected CanvasGroup canvasGroup;

    [SerializeField] protected Button closeButton;

    [SerializeField] protected float slideDistance = 420f;

    [SerializeField] protected float slideDuration = 0.25f;

    public Action<ToastNotificationBase> OnToastClosed;

    // Notification Data
    protected ToastData currentData;

    protected bool isClosing;

    private Coroutine slideCoroutine;

    // Lifecycle
    protected virtual void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Dismiss);
        }
    }

    protected virtual void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Dismiss);
        }
    }

    public virtual void Show(ToastData data)
    {
        currentData = data;
        isClosing = false;

        ApplyData(data);

        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(data.closable);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (visualPanel != null)
        {
            Vector2 startPos = visualPanel.anchoredPosition;
            startPos.x = slideDistance;
            visualPanel.anchoredPosition = startPos;
        }

        StopSlideCoroutine();
        slideCoroutine = StartCoroutine(SlideInRoutine());
    }

    public virtual void Dismiss()
    {
        if (isClosing || !gameObject.activeInHierarchy)
        {
            return;
        }

        isClosing = true;
        OnToastClosing();

        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        StopSlideCoroutine();
        slideCoroutine = StartCoroutine(SlideOutRoutine());
    }

    public virtual void ForceCloseImmediate()
    {
        isClosing = true;
        OnToastClosing();
        StopSlideCoroutine();

        OnToastClosed?.Invoke(this);
    }

    public virtual void ResetForPool()
    {
        StopAllCoroutines();
        slideCoroutine = null;
        isClosing = false;
        currentData = null;

        if (visualPanel != null)
        {
            Vector2 pos = visualPanel.anchoredPosition;
            pos.x = 0f;
            visualPanel.anchoredPosition = pos;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    // Slide in animation
    private IEnumerator SlideInRoutine()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, slideDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easeOut = 1f - Mathf.Pow(1f - t, 3f);

            if (visualPanel != null)
            {
                Vector2 pos = visualPanel.anchoredPosition;
                pos.x = Mathf.Lerp(slideDistance, 0f, easeOut);
                visualPanel.anchoredPosition = pos;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = easeOut;
            }

            yield return null;
        }

        if (visualPanel != null)
        {
            Vector2 endPos = visualPanel.anchoredPosition;
            endPos.x = 0f;
            visualPanel.anchoredPosition = endPos;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        slideCoroutine = null;
        OnToastShown();
    }

    // Slide out animation
    private IEnumerator SlideOutRoutine()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, slideDuration * 0.8f);
        float startX = visualPanel != null ? visualPanel.anchoredPosition.x : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easeIn = t * t;

            if (visualPanel != null)
            {
                Vector2 pos = visualPanel.anchoredPosition;
                pos.x = Mathf.Lerp(startX, slideDistance, easeIn);
                visualPanel.anchoredPosition = pos;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f - easeIn;
            }

            yield return null;
        }

        slideCoroutine = null;


        OnToastClosed?.Invoke(this);
    }

    private void StopSlideCoroutine()
    {
        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
            slideCoroutine = null;
        }
    }

    // Lifecycle hooks for subclasses
    protected abstract void ApplyData(ToastData data);

    protected abstract void OnToastShown();

    protected virtual void OnToastClosing()
    {
    }
}
