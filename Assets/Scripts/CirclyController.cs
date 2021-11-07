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
    [SerializeField] private float fleeRadius;       // distance at which circle starts to run away from cursor
    [SerializeField] private float safeDistance;     // distance at which fleeing circle "calms down" and heads to finish
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float baseAcceleration;

    public bool started = false;
    public UnityEvent finishEvent;                   // invokes when circle reaches finish area

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        transform.position = startPosition;
        fleeSpeed = baseSpeed * 1.5f;
        fleeAcceleration = baseAcceleration * 10;
        fleeRadius = GetComponent<SphereCollider>().bounds.extents.x + scaryCursor.GetComponent<SphereCollider>().bounds.extents.x;
        safeDistance = fleeRadius * 1.5f;
    }

    void Update()
    {
        if (!started) return;

        UpdateMovement();
    }

    /// <summary>
    /// Defining whether circle should run away from cursor or head to the finish area
    /// </summary>
    private void UpdateMovement()
    {
        if (Vector3.Distance(transform.position, scaryCursor.position) > safeDistance)
        {
            if (isFleeing)
                HeadToFinish();
        }
        else if (isFleeing || Vector3.Distance(transform.position, scaryCursor.position) < fleeRadius)
        {
            FleeAway();
        }
    }

    /// <summary>
    /// Calculating position where circle should run from cursor.
    /// </summary>
    private void FleeAway() {
        // if circle wasn't fleeing before, increase the speed and acceleration
        if (!isFleeing)
        {
            isFleeing = true;
            agent.speed = fleeSpeed;
            agent.acceleration = fleeAcceleration;
        }
        Vector3 newDestination = transform.position + (transform.position - scaryCursor.position).normalized * safeDistance;
        agent.SetDestination(newDestination);
    }

    /// <summary>
    /// Settings base speed and acceleration and setting destination point to finish area
    /// </summary>
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
        safeDistance = fleeRadius * 1.5f;
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
        // if circle reached the finish area
        if (other.gameObject.layer == finishArea.gameObject.layer)
        {
            started = false;
            isFleeing = false;

            // set up the circle to smoothly enter the finish area
            agent.velocity = Vector3.zero;
            agent.acceleration = 2;
            agent.SetDestination(finishArea.position);

            // invoke an event
            finishEvent.Invoke();
        }
    }

    /// <summary>
    /// Setting base speed and acceleration and putting circle to start position. Called before starting the game
    /// </summary>
    public void Reset()
    {
        transform.position = startPosition;
        agent.speed = baseSpeed;
        agent.acceleration = baseAcceleration;
        agent.velocity = Vector3.zero;
        agent.ResetPath();
    }
}
