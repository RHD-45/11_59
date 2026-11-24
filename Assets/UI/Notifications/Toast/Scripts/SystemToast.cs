using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemToast : ToastNotificationBase, IAutoDismissToast
{
    // Toast Configurations
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Image logoImage;

    // Logo
    [SerializeField] private Sprite windowLogo;
    [SerializeField] private Sprite warningLogo;
    [SerializeField] private Sprite hcmutLogo;
    [SerializeField] private Sprite zaloLogo;

    private float autoDismissDuration;

    private Coroutine autoDismissCoroutine;

    public float AutoDismissDuration
    {
        get
        {
            return autoDismissDuration;
        }
    }

    protected override void ApplyData(ToastData data)
    {
        if (titleText != null)
        {
            titleText.text = data.title;
        }

        if (messageText != null)
        {
            messageText.text = data.message;
        }

        if (logoImage != null)
        {
            Sprite targetLogo = GetLogoSprite(data);
            logoImage.sprite = targetLogo;
            logoImage.enabled = targetLogo != null;
        }

        autoDismissDuration = data.duration;
    }

    protected override void OnToastShown()
    {
        if (autoDismissDuration > 0f)
        {
            StartAutoDismiss(autoDismissDuration);
        }
    }

    public void StartAutoDismiss(float duration)
    {
        StopAutoDismiss();
        autoDismissDuration = duration;
        autoDismissCoroutine = StartCoroutine(AutoDismissRoutine(duration));
    }

    protected override void OnToastClosing()
    {
        StopAutoDismiss();
    }

    public override void ResetForPool()
    {
        StopAutoDismiss();
        autoDismissDuration = 0f;
        base.ResetForPool();
    }


    private IEnumerator AutoDismissRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        autoDismissCoroutine = null;
        Dismiss();
    }

    // Stop auto dismiss coroutine if exists
    private void StopAutoDismiss()
    {
        if (autoDismissCoroutine != null)
        {
            StopCoroutine(autoDismissCoroutine);
            autoDismissCoroutine = null;
        }
    }


    // Logo selection
    private Sprite GetLogoSprite(ToastData data)
    {
        switch (data.type)
        {
            case ToastType.PowerCutWarning:
                return warningLogo;
            case ToastType.Assignment:
                return hcmutLogo;
            case ToastType.Zalo:
                return zaloLogo;
            default:
                return windowLogo;
        }
    }
}
