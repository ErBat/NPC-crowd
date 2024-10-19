using UnityEngine;
using UnityEngine.AI;

public class WaitAtCrosswalkBehaviour : MonoBehaviour, IAgentBehaviour
{
    private NavMeshAgent agent;
    private PedestrianSpawner spawner;
    private TrafficLightController trafficLight;
    private bool waitingAtCrosswalk = false;
    private Transform destination;

    public void Initialize(NavMeshAgent agent, PedestrianSpawner spawner)
    {
        this.agent = agent;
        this.spawner = spawner;
        this.trafficLight = FindObjectOfType<TrafficLightController>(); 
    }

    public void SetDestination(Transform destination)
    {
        this.destination = destination;
        agent.SetDestination(destination.position);
    }

    public void Execute()
    {
        if (waitingAtCrosswalk)
        {
            if (trafficLight != null && trafficLight.IsGreenLight())
            {
                agent.isStopped = false; 
                waitingAtCrosswalk = false;
            }
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance <= 1.0f)
            {
                spawner.DespawnAgent(gameObject); 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered with: " + other.gameObject.name + ", Tag: " + other.tag);

        if (other.CompareTag("CrosswalkZone") && trafficLight != null && trafficLight.currentLightState == TrafficLightController.LightState.Red)
        {
            agent.isStopped = true;
            waitingAtCrosswalk = true;
        }
    }
}
