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
    private NavMeshAgent agent;
    public TextMeshProUGUI debugText;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(finishArea.position);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            debugText.text = hit.point.ToString();
            if (Vector3.Distance(transform.position, hit.point) < fleeRadius)
            {
                Vector3 newDestination = transform.position * 2 - hit.point;
                agent.SetDestination(newDestination);
            }
        }
        /*Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        mousePosition.y = 0;
        debugText.text = mousePosition.ToString();
        if (Vector3.Distance(transform.position, mousePosition) < fleeRadius)
        {
            Vector3 newDestination = transform.position * 2 - mousePosition;
            agent.SetDestination(newDestination);
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & finishAreaLayer) != 0)
        {
            Debug.Log("Finish trigger");
        }
    }
}
