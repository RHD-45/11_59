using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AssignmentSpawner : MonoBehaviour
{
    [SerializeField] private AssignmentTracker tracker;
    [SerializeField] private AssignmentDefinition[] taskList;   // cài sẵn
    [SerializeField] private float minTimeBetweenSpawn;
    [SerializeField] private float maxRandomDelay;
    [SerializeField] private float spawnerTimer;
    private int assignmentsSpawned = 0;

    private Coroutine spawnRoutine;

    public void StartRun()
    {
        StopRun();
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
        while (assignmentsSpawned < taskList.Length)
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
        if (assignmentsSpawned >= taskList.Length) return;

        AssignmentDefinition definition = taskList[assignmentsSpawned];
        assignmentsSpawned++;
        SpawnAssignment(definition);
    }
    
    public void SpawnAssignment(AssignmentDefinition assignment)
    {
        AssignmentState state = tracker.CreateNewAssignment(assignment);

        GameObject window = Instantiate(assignment.prefab);
    }
}
