using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject scaryCursor;     // object which the circle runs away from. Syncs position with the cursor
    [SerializeField] private CirclyController circly;
    [SerializeField] private GameObject gameoverWindow;
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        UpdateScaryCursor();
    }

    /// <summary>
    /// Updating scaryCursor object's position and visibility according to cursor position
    /// </summary>
    private void UpdateScaryCursor()
    {
        if (!circly.started)
        {
            scaryCursor.SetActive(false);
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // if cursor is on the arena, then sync object's position and make it visible
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            scaryCursor.SetActive(true);
            scaryCursor.transform.position = hit.point;
        }
        else
        {
            scaryCursor.SetActive(false);
        }
    }

    public void StartGame()
    {
        circly.Reset();
        circly.started = true;
        circly.HeadToFinish();
        gameoverWindow.SetActive(false);
    }

    public void FinishGame()
    {
        circly.started = false;
        gameoverWindow.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnCursorScaleChanged(float newValue)
    {
        scaryCursor.transform.localScale = Vector3.one * newValue;
        var sh = scaryCursor.GetComponentInChildren<ParticleSystem>().shape;
        sh.radius = newValue;
    }
}
