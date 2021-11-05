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

    [SerializeField] private int baseAcceleration;
    [SerializeField] private int baseSpeed;
    [SerializeField] private int runSpeed;
    [SerializeField] private int runAcceleration;
    private bool headingToFinish = false;

    private bool started = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!started) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, Mathf.Infinity, groundLayer))
        {
            if (Vector3.Distance(transform.position, scaryCursor.position) > fleeDistance)
            {
                if (!headingToFinish)
                    HeadToFinish();
            }
            else if (Vector3.Distance(transform.position, scaryCursor.position) < fleeRadius)
            {
                RunAway(scaryCursor.position);
            }
        }
    }

    private void RunAway(Vector3 scaryPoint) {
        headingToFinish = false;

        agent.speed = runSpeed;
        agent.acceleration = runAcceleration;

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
        headingToFinish = true;
        /*agent.velocity = Vector3.zero;*/
        agent.speed = baseSpeed;
        agent.acceleration = baseAcceleration;
        agent.SetDestination(finishArea.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & finishAreaLayer) != 0)
        {
            Debug.Log("Finish trigger");
        }
    }

    public void ChangeFleeRadius(float newValue)
    {
        fleeRadius = newValue;
    }
}
