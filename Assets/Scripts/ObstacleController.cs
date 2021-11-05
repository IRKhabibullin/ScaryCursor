using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            transform.position = hit.point;
        }
    }

    public void OnScaleChanged(float newValue)
    {
        transform.localScale = Vector3.one * newValue;
    }
}
