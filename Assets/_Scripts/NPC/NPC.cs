using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC : CharacterBase
{
    [Header("AI")]
    public NPC_Controller _controller;
    [SerializeField] private Transform wayPoint;

    [Header("Speed")]
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float maxDistanceWithController = 2f;
    [SerializeField] private float sprintDistance;
    [SerializeField] private Vector2 movement;

    private NavMeshAgent agent;
    internal HitBox target;

    private void OnValidate()
    {
        agent ??= GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _controller ??= GetComponent<NPC_Controller>();
        agent.speed = movementSpeed;
    }

    private void Update()
    {
        if (!_controller.hitBox.Alive())
        {
            return;
        }

        if(wayPoint == null)
        {
            return;
        }

        agent.destination = wayPoint.position;

        float distanceToWayPoint = Vector3.Distance(transform.position, wayPoint.position);
        float DistanceToController = Vector3.Distance(transform.position, _controller.transform.position);

        if (agent.velocity.magnitude < 0.1f && distanceToWayPoint < agent.stoppingDistance && DistanceToController < maxDistanceWithController)
        {
            movement.y = 0;
            movement.x = 0;
            _controller.receiver.movementInput = movement;
            _controller.receiver.sprintInput = false;
            return;
        }

        SpeedControl();
        ShouldSprint();

        Vector3 direction = transform.position - _controller.transform.position;
        direction.Normalize();

        movement.x = Vector3.Dot(transform.right, direction);

        float forwardAmount = agent.remainingDistance > 0 ? 1 : 0;
        movement.y = Vector3.Dot(transform.forward, direction) * forwardAmount;

        movement.y = Mathf.Abs(movement.y) > 0.05 ? movement.y : 0;

        _controller.receiver.movementInput = movement;
        _controller.Direction(direction);
    }

    public void MoveTo(Vector3 position)
    {
        wayPoint.position = position;
    }

    private void SpeedControl()
    {
        float distance = Vector3.Distance(transform.position, _controller.transform.position);
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
            _controller.receiver.sprintInput = true;
        }
        else
        {
            _controller.receiver.sprintInput = false;
        }
    }

    public void Follow(Transform point)
    {
        wayPoint = point;
    }

    public void Attack(HitBox hitbox)
    {
        
    }

    public void Scan()
    {
        
    }

}