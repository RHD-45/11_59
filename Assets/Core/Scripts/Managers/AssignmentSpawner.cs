using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AssignmentSpawner : MonoBehaviour
{
    [SerializeField] private Transform windowParent;
    [SerializeField] private AssignmentTracker tracker;
    [SerializeField] private List<AssignmentDefinition> taskList = new List<AssignmentDefinition>();   // cài sẵn
    [SerializeField] private float minTimeBetweenSpawn;
    [SerializeField] private float maxRandomDelay;
    private float spawnerTimer;
    private int assignmentsSpawned = 0;

    private Coroutine spawnRoutine;

    public void StartRun()
    {
        StopRun();
        assignmentsSpawned = 0;
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void StopRun()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (assignmentsSpawned < taskList.Count)
        {
            float randomDelay = Random.Range(0f, maxRandomDelay);
            float waitTime = minTimeBetweenSpawn + randomDelay;

            yield return new WaitForSeconds(waitTime);

            SpawnNextAssignment();
        }

        spawnRoutine = null;
    }

    private void SpawnNextAssignment()
    {
        if (assignmentsSpawned >= taskList.Count) return;

        AssignmentDefinition definition = taskList[assignmentsSpawned];
        assignmentsSpawned++;
        SpawnAssignment(definition);
    }
    
    public void SpawnAssignment(AssignmentDefinition assignment)
    {
        AssignmentState state = tracker.CreateNewAssignment(assignment);

        GameObject window = Instantiate(assignment.prefab, windowParent);

        Demo demo = window.GetComponent<Demo>();
        demo.Initialize(state.InstanceId, tracker);
    }
}
