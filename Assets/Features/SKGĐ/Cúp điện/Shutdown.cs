using UnityEngine;
using System.Collections;

public class Shutdown : MonoBehaviour
{
    [SerializeField] private InterruptionDefinition definition;
    [SerializeField] private InterruptionManager manager;
    [SerializeField] private TypeWriterText typeWriterText;
    [SerializeField] private float warningDuration = 2f;
    private Coroutine warningCoroutine;

    private void OnEnable()
    {
        manager.OnInterruptionEnding += HandleInterruptionEnding;
    }

    private void OnDisable()
    {
        manager.OnInterruptionEnding -= HandleInterruptionEnding;
    }

    [ContextMenu("Test Power Cut")]
    public void StartShutdownInterruption()
    {
        if (warningCoroutine != null)
        {
            return;
        }

        warningCoroutine = StartCoroutine(ShutdownRoutine());
    }

    private IEnumerator ShutdownRoutine()
    {
        if (GameManager.State != GameState.Playing)
        {
            warningCoroutine = null;
            yield break;
        }

        EventBroker.onToastRequested?.Invoke(
            new ToastData(
                ToastType.PowerCutWarning,
                "CẢNH BÁO ĐIỆN ÁP",
                "Nguồn điện không ổn định!",
                warningDuration,
                false
            )
        );

        yield return new WaitForSeconds(
            warningDuration
        );

        manager.StartInterruption(definition);

        if (GameManager.State == GameState.Playing)
        {
            GameManager.Instance.ChangeState(
                GameState.Interrupted
            );
        }

        warningCoroutine = null;
    }

    private void HandleInterruptionEnding(InterruptionDefinition definition)
    {
        typeWriterText.RestoreLastCheckpoint();
    }
}
