using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform playerTransform;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }


    void Update()
    {
        if (playerTransform != null)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;

            Quaternion rotation = Quaternion.LookRotation(-directionToPlayer);

            transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        }
    }
}
