using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CirclyController : MonoBehaviour
{
    [SerializeField] private LayerMask finishAreaLayer;
    [SerializeField] private Transform finishArea;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(finishArea.position);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & finishAreaLayer) != 0)
        {
            Debug.Log("Finish trigger");
        }
    }
}
