using System.Collections;
using UnityEngine;
using System;

public class InterruptionManager : MonoBehaviour
{
    [SerializeField] private GameObject inputBlocker;       // chắn full màn hình
    [SerializeField] private Transform interruptionCanvas;
    private bool isInterrupting = false;
    private Coroutine coroutine;
    private InterruptionDefinition currentDefinition;
    private GameObject activeInterruption;
    public event Action<InterruptionDefinition> OnInterruptionEnding; // load word các thứ
    public event Action<InterruptionDefinition> OnInterruptionFinished;   // continue playing

    private void Awake()
    {
        inputBlocker.SetActive(false);
    }

    public void StartInterruption(InterruptionDefinition definition)
    {
        if (isInterrupting) return;
        coroutine = StartCoroutine(PlayCoroutine(definition));
    }
    private IEnumerator PlayCoroutine(InterruptionDefinition definition)
    {
        currentDefinition = definition;
        inputBlocker.SetActive(true);
        activeInterruption = Instantiate(definition.prefab, interruptionCanvas);
        yield return new WaitForSeconds(definition.duration);

        OnInterruptionEnding?.Invoke(definition);
        EndInterruption();
        OnInterruptionFinished?.Invoke(definition);
    }

    private void EndInterruption()
    {
        inputBlocker.SetActive(false);
        Destroy(activeInterruption);

        activeInterruption = null;
        currentDefinition = null;
        coroutine = null;
    }
    
}
