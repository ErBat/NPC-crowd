using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PedestiranPooling : MonoBehaviour
{
    public GameObject agentPrefab;
    public int maxAgents = 10;
    public float navMeshSampleDistance = 50f;
    private float speedCoefficent = 1.5f;

    private Queue<GameObject> agentPool = new Queue<GameObject>();
    private List<GameObject> activeAgents = new List<GameObject>();
    void Start()
    {
        for (int i = 0; i < maxAgents; i++)
        {
            GameObject agent = Instantiate(agentPrefab);
            agent.SetActive(false);
            agentPool.Enqueue(agent);
        }

        SpawnAgentsIfNeeded();
    }

    void Update()
    {
        SpawnAgentsIfNeeded();

        for (int i = activeAgents.Count - 1; i >= 0; i--)
        {
            NavMeshAgent navAgent = activeAgents[i].GetComponent<NavMeshAgent>();
            if (!navAgent.pathPending && navAgent.remainingDistance <= 1.0f)
            {
                DespawnAgent(activeAgents[i]);
            }
        }
    }

    private void SpawnAgentsIfNeeded()
    {
        while (activeAgents.Count < maxAgents && agentPool.Count > 0)
        {
            GameObject agent = agentPool.Dequeue();
            agent.transform.position = GetRandomNavMeshPoint();
            agent.SetActive(true);

            NavMeshAgent navAgent = agent.GetComponent<NavMeshAgent>();
            navAgent.SetDestination(GetRandomNavMeshPoint());

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
