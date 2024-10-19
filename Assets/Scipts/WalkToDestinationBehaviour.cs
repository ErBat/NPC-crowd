using UnityEngine;
using UnityEngine.AI;

public class WalkToDestinationBehaviour : MonoBehaviour, IAgentBehaviour
{
    private NavMeshAgent agent;
    private PedestrianSpawner spawner;
    private Transform destination;

    public void Initialize(NavMeshAgent agent, PedestrianSpawner spawner)
    {
        this.agent = agent;
        this.spawner = spawner;
    }

    public void SetDestination(Transform destination)
    {
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is not initialized. Ensure Initialize() is called before SetDestination().");
            return;
        }

        this.destination = destination;
        agent.SetDestination(destination.position);
    }

    public void Execute()
    {
        if (agent != null && !agent.pathPending && agent.remainingDistance <= 1.0f)
        {
            spawner.DespawnAgent(gameObject);
        }
    }
}
