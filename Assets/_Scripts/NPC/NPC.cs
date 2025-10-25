using UnityEngine;
using UnityEngine.AI;
/*
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class NPC : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 direction;
    private Vector2 groundDeltaPosition;
    private Vector2 velocity;

    private void Awake()
    {
        agent ??= GetComponent<NavMeshAgent>();
        animator ??= GetComponent<Animator>();
        agent.updatePosition = false;
    }

    private void LateUpdate()
    {
        Animate();
    }

    private void Animate()
    {

        direction = agent.nextPosition - transform.position;
        groundDeltaPosition.x = Vector3.Dot(transform.right, direction);
        groundDeltaPosition.y = Vector3.Dot(transform.forward, direction);
        velocity = (Time.deltaTime > 1e-5f) ? groundDeltaPosition / Time.deltaTime : Vector2.zero;
        bool shouldMove = velocity.magnitude > 0.025f && agent.remainingDistance > agent.radius;

        animator.SetBool("move", shouldMove);
        animator.SetFloat("Vertical", velocity.y);
        animator.SetFloat("Horizontal", velocity.x);
    }

    private void OnAnimatorMove()
    {
        transform.position = agent.nextPosition;
    }
}*/

[RequireComponent(typeof(NavMeshAgent))]
public class NPC : CharacterBase
{
    [Header("AI")]
    [SerializeField] private Transform wayPoint;

    [Header("Speed")]
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float maxDistanceWithController = 2f;
    [SerializeField] private float sprintDistance;

    private NavMeshAgent agent;
    [SerializeField] private Vector2 movement;
    private Vector2 groundDeltaPosition;

    private void OnValidate()
    {
        agent ??= GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!controller.hitBox.Alive())
        {
            return;
        }

        agent.destination = wayPoint.position;

        float distanceToWayPoint = Vector3.Distance(transform.position , wayPoint.position);
        float DistanceToController = Vector3.Distance(transform.position, controller.transform.position);

        if (agent.velocity.magnitude < 0.1f && distanceToWayPoint < agent.stoppingDistance && DistanceToController < maxDistanceWithController)
        {
            movement.y = 0;
            movement.x = 0;
            controller.receiver.movementInput = movement;
            controller.receiver.sprintInput = false;
            return;
        }

        SpeedControl();
        ShouldSprint();

        Vector3 direction = transform.position - controller.transform.position;
        direction.Normalize();
        
        movement.x = Vector3.Dot(transform.right, direction);

        float forwardAmount = agent.remainingDistance > 0 ? 1 : 0;
        movement.y = Vector3.Dot(transform.forward, direction) * forwardAmount;

        movement.y = Mathf.Abs(movement.y) > 0.05 ? movement.y : 0;

        controller.receiver.movementInput = movement;
        controller.Direction(direction);
    }


    private void SpeedControl()
    {
        float distance = Vector3.Distance(transform.position, controller.transform.position);
        if (distance > maxDistanceWithController)
        {
            agent.speed = 0;
        }
        else
        {
            agent.speed = movementSpeed;
        }
    }
    
    private void ShouldSprint()
    {
        if (Vector3.Distance(transform.position, wayPoint.position) > sprintDistance)
        {
            controller.receiver.sprintInput = true;
        }
        else
        {
            controller.receiver.sprintInput = false;
        }
    }

}