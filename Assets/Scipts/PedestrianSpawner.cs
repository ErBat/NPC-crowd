using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class PedestrianSpawner : MonoBehaviour
{
    public GameObject agentPrefab;
    public int maxAgents = 10;
    public Transform startPoint;
    public Transform endPoint;
    public Transform endPoint2;
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
        SpawnAgentsIfNeeded();
        foreach (GameObject agent in activeAgents)
        {
            IAgentBehaviour behaviour = agent.GetComponent<IAgentBehaviour>();
            behaviour?.Execute();
        }
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
            Vector3 spawnPosition = spawnPoint.position;

            Vector3 widthOffset = Vector3.Cross((endPoint.position - startPoint.position).normalized, Vector3.up);
            spawnPosition += widthOffset * Random.Range(-sidewalkWidth / 2f, sidewalkWidth / 2f);

            agent.transform.position = spawnPosition;
            agent.SetActive(true);

            AssignBehavior(agent, spawnPoint);
            activeAgents.Add(agent);
        }
    }

    private void AssignBehavior(GameObject agent, Transform spawnPoint)
    {
        if (Random.value > 0.5f)
        {
            WalkToDestinationBehaviour walkBehaviour = agent.AddComponent<WalkToDestinationBehaviour>();
            walkBehaviour.Initialize(agent.GetComponent<NavMeshAgent>(), this);

            Transform destination = spawnPoint == startPoint ? endPoint : startPoint;
            walkBehaviour.SetDestination(destination);
        }
        else
        {
            WaitAtCrosswalkBehaviour waitBehaviour = agent.AddComponent<WaitAtCrosswalkBehaviour>();
            waitBehaviour.Initialize(agent.GetComponent<NavMeshAgent>(), this);

            Transform destination = endPoint2;
            waitBehaviour.SetDestination(destination);
        }
    }



    public void DespawnAgent(GameObject agent)
    {
        agent.SetActive(false);
        activeAgents.Remove(agent);
        agentPool.Enqueue(agent);
    }
}
