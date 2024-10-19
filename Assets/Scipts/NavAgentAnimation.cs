using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAgentAnimation : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;
    private float speedCoefficent = 1.5f;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = Random.Range(1f, 1.2f);
    }

    void Update()
    {
        float speed = agent.velocity.magnitude * speedCoefficent;

        animator.SetFloat("Speed", speed);
        animator.SetFloat("MotionSpeed", speed);
    }

    public void OnFootstep()
    {
        // Code to handle footstep sounds or logic
    }
}
