using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] private string TriggerTag = "Player";
    public Vector3 detectionBoxSize = new Vector3(2f, 2f, 2f);
    public UnityEvent onPlayerTouch;


    void Update()
    {
        bool isPlayerInside = CheckPlayerInside();

        // Do something based on the detection result
        if (isPlayerInside)
        {
            Debug.Log("Player is inside the detection box!");
            // Add your logic here
            onPlayerTouch.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a wireframe box to visualize the detection box in the Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, detectionBoxSize);

        // Draw a solid box if the player is inside the detection box during editing
        if (CheckPlayerInside())
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f); // Yellow with transparency
            Gizmos.DrawCube(transform.position, detectionBoxSize);
        }
    }

    private bool CheckPlayerInside()
    {
        // Perform a BoxCast to check if the player is inside the box
        Collider[] hitColliders = Physics.OverlapBox(transform.position, detectionBoxSize / 2, Quaternion.identity);

        // Check if any of the colliders have the specified tag
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag(TriggerTag))
            {
                return true;
            }
        }

        return false;
    }
}
