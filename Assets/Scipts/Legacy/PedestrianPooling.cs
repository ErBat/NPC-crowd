using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class PedestrianPooling : MonoBehaviour
{
    public GameObject agentPrefab;
    public int maxAgents = 10;
    public float navMeshSampleDistance = 50f;
    public Transform startPoint;
    public Transform endPoint;
    public float sidewalkLength = 50f;
    public float sidewalkWidth = 2f;

    private Queue<GameObject> agentPool = new Queue<GameObject>();
    private List<GameObject> activeAgents = new List<GameObject>();
    private bool isSpawning = false;


    void Start()
    {
        for (int i = 0; i < maxAgents; i++)
        {
            GameObject agent = Instantiate(agentPrefab);
            agent.SetActive(false);
            agentPool.Enqueue(agent);
        }

        StartCoroutine(SpawnAgentsGradually());
    }

    void Update()
    {
        for (int i = activeAgents.Count - 1; i >= 0; i--)
        {
            NavMeshAgent navAgent = activeAgents[i].GetComponent<NavMeshAgent>();
            if (!navAgent.pathPending && navAgent.remainingDistance <= 1.0f)
            {
                DespawnAgent(activeAgents[i]);
            }
        }

        SpawnAgentsIfNeeded();
    }

    private IEnumerator SpawnAgentsGradually()
    {
        while (activeAgents.Count < maxAgents && agentPool.Count > 0)
        {
            SpawnAgent();
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private void SpawnAgentsIfNeeded()
    {
        if (activeAgents.Count < maxAgents && agentPool.Count > 0 && !isSpawning)
        {
            StartCoroutine(SpawnAgentWithDelay());
        }
    }

    private IEnumerator SpawnAgentWithDelay()
    {
        isSpawning = true;
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        SpawnAgent();
        isSpawning = false;
    }

    private void SpawnAgent()
    {
        if (agentPool.Count > 0)
        {
            GameObject agent = agentPool.Dequeue();

            Transform spawnPoint = Random.value > 0.5f ? startPoint : endPoint;
            Transform destinationPoint = spawnPoint == startPoint ? endPoint : startPoint;

            Vector3 spawnPosition = spawnPoint.position;

            // Add a random offset along the width direction
            Vector3 widthOffset = Vector3.Cross((endPoint.position - startPoint.position).normalized, Vector3.up);
            spawnPosition += widthOffset * Random.Range(-sidewalkWidth / 2f, sidewalkWidth / 2f);

            agent.transform.position = spawnPosition;
            agent.SetActive(true);

            NavMeshAgent navAgent = agent.GetComponent<NavMeshAgent>();
            navAgent.SetDestination(destinationPoint.position);

            activeAgents.Add(agent);
        }
    }

    private void DespawnAgent(GameObject agent)
    {
        agent.SetActive(false);
        activeAgents.Remove(agent);
        agentPool.Enqueue(agent);
    }

    private Vector3 GetRandomNavMeshPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * navMeshSampleDistance;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }
}
