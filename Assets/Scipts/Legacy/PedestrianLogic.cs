using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianLogic : MonoBehaviour
{
    public Transform target;
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;
    private float speedCoefficent = 1.5f;
    public float navMeshSampleDistance = 50f; 
    private float distanceThreshold = 1.0f; 

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.SetDestination(target.position);
        animator = GetComponent<Animator>();
        MoveToFarthestPoint();
    }

    void Update()
    {
        float speed = agent.velocity.magnitude * speedCoefficent;

        animator.SetFloat("Speed", speed);
        animator.SetFloat("MotionSpeed", speed);

        if (!agent.pathPending && agent.remainingDistance <= distanceThreshold)
        {
            MoveToFarthestPoint();
        }
    }

    private void MoveToFarthestPoint()
    {
        Vector3 farthestPoint = FindMostDistantNavMeshPoint();
        if (farthestPoint != Vector3.zero)
        {
            agent.SetDestination(farthestPoint);
        }
    }

    private Vector3 FindMostDistantNavMeshPoint()
    {
        Vector3 currentPosition = transform.position;
        Vector3 farthestPoint = Vector3.zero;
        float maxDistance = 0;

        for (int i = 0; i < 10; i++) 
        {
            Vector3 randomDirection = Random.insideUnitSphere * navMeshSampleDistance;
            randomDirection += currentPosition;

            if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out UnityEngine.AI.NavMeshHit hit, navMeshSampleDistance, UnityEngine.AI.NavMesh.AllAreas))
            {
                float distance = Vector3.Distance(currentPosition, hit.position);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPoint = hit.position;
                }
            }
        }

        return farthestPoint;
    }
}
