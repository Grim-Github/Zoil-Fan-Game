using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private string TriggerTag = "Player";

    [Header("Detection Object")]
    public Vector3 detectionBoxSize = new Vector3(2f, 2f, 2f);

    [Header("Detection Events")]
    public UnityEvent onObjectTouch;
    public UnityEvent onObjectExit;
    public UnityEvent onObjectEnter;


    private bool objectEntered = false;


    void Update()
    {
        bool isPlayerInside = CheckPlayerInside();

        // Do something based on the detection result
        if (isPlayerInside)
        {
            Debug.Log("Player is inside the detection box!");

            onObjectTouch.Invoke();

            if(objectEntered == false)
            {
                objectEntered = true;
                onObjectEnter.Invoke();
            }
        }
        else
        {
            if(objectEntered == true)
            {
                objectEntered = false;
                onObjectExit.Invoke();
            }
        }
    }

    public void LogTrigger(string text)
    {
        Debug.Log(text);
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
