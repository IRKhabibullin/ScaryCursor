using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class CirclyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private float fleeSpeed;
    private float fleeAcceleration;
    private bool isFleeing = false;

    [SerializeField] private Transform scaryCursor;
    [SerializeField] private Transform finishArea;
    [SerializeField] private float fleeRadius;
    [SerializeField] private float fleeDistance;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float baseAcceleration;

    public bool started = false;
    public UnityEvent finishEvent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        transform.position = startPosition;
        fleeSpeed = baseSpeed * 1.5f;
        fleeAcceleration = baseAcceleration * 10;
        fleeRadius = GetComponent<SphereCollider>().bounds.extents.x + scaryCursor.GetComponent<SphereCollider>().bounds.extents.x;
        fleeDistance = fleeRadius * 1.5f;
    }

    void Update()
    {
        if (!started) return;

        if (isFleeing && Vector3.Distance(transform.position, scaryCursor.position) > fleeDistance)
        {
            HeadToFinish();
        }
        else if (Vector3.Distance(transform.position, scaryCursor.position) < fleeRadius)
        {
            FleeAway(scaryCursor.position);
        }
    }

    private void FleeAway(Vector3 scaryPoint) {
        if (!isFleeing)
        {
            isFleeing = true;
            agent.speed = fleeSpeed;
            agent.acceleration = fleeAcceleration;
        }
        Vector3 newDestination = transform.position + (transform.position - scaryPoint).normalized * fleeDistance;
        agent.SetDestination(newDestination);
    }

    public void HeadToFinish()
    {
        agent.speed = baseSpeed;
        agent.acceleration = baseAcceleration;
        agent.SetDestination(finishArea.position);
        isFleeing = false;
    }

    #region sliders handlers
    public void ChangeFleeRadius(float newValue)
    {
        fleeRadius = GetComponent<SphereCollider>().bounds.extents.x + newValue / 2;
        fleeDistance = fleeRadius * 1.5f;
    }

    public void OnSpeedChanged(float newValue)
    {
        baseSpeed = newValue;
        fleeSpeed = newValue * 1.5f;
        agent.speed = isFleeing ? fleeSpeed : baseSpeed;
    }

    public void OnAccelerationChanged(float newValue)
    {
        baseAcceleration = newValue;
        fleeAcceleration = newValue * 10;
        agent.acceleration = isFleeing ? fleeAcceleration : baseAcceleration;
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == finishArea.gameObject.layer)
        {
            finishEvent.Invoke();
        }
    }

    public void Reset()
    {
        transform.position = startPosition;
        agent.speed = baseSpeed;
        agent.acceleration = baseAcceleration;
        agent.velocity = Vector3.zero;
        agent.ResetPath();
    }
}
