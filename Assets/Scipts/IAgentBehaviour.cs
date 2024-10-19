using UnityEngine.AI;

public interface IAgentBehaviour
{
    void Initialize(NavMeshAgent agent, PedestrianSpawner spawner);
    void Execute();
}

