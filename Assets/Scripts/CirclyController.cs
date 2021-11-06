using UnityEngine;
using UnityEngine.AI;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
public class CirclyController : MonoBehaviour
{
    [SerializeField] private LayerMask finishAreaLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform finishArea;
    [SerializeField] private float fleeRadius;
    [SerializeField] private float fleeDistance;
    [SerializeField] private Transform scaryCursor;
    private NavMeshAgent agent;
    public TextMeshProUGUI debugText;

    [SerializeField] private float baseAcceleration;
    [SerializeField] private float baseSpeed;
    private float fleeSpeed;
    private float fleeAcceleration;

    private bool started = false;
    private bool isFleeing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fleeRadius = GetComponent<SphereCollider>().bounds.extents.x + scaryCursor.GetComponent<SphereCollider>().bounds.extents.x;
        fleeDistance = fleeRadius * 1.5f;
        fleeSpeed = baseSpeed * 1.5f;
        fleeAcceleration = baseAcceleration * 10;
    }

    void Update()
    {
        if (!started) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, Mathf.Infinity, groundLayer))
        {
            if (isFleeing && Vector3.Distance(transform.position, scaryCursor.position) > fleeDistance)
            {
                HeadToFinish();
            }
            else if (Vector3.Distance(transform.position, scaryCursor.position) < fleeRadius)
            {
                FleeAway(scaryCursor.position);
            }
        }
        debugText.text = agent.destination.ToString();
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

        Debug.DrawLine(transform.position, newDestination, Color.red, 0);
    }

    public void StartGame()
    {
        started = true;
        HeadToFinish();
    }

    public void HeadToFinish()
    {
        agent.speed = baseSpeed;
        agent.acceleration = baseAcceleration;
        agent.SetDestination(finishArea.position);
        isFleeing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & finishAreaLayer) != 0)
        {
            Debug.Log("Finish trigger");
        }
    }

    public void ChangeFleeRadius()
    {
        fleeRadius = GetComponent<SphereCollider>().bounds.extents.x + scaryCursor.GetComponent<SphereCollider>().bounds.extents.x;
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
}
