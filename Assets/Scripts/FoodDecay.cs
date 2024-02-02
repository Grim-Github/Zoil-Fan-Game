using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDecay : MonoBehaviour
{
    // Adjust this decay speed based on your needs
    public float decaySpeed = 1.0f;

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * decaySpeed);

        if (transform.localScale.magnitude < 0.01f)
        {
            Destroy(gameObject);
        }
    }
}
