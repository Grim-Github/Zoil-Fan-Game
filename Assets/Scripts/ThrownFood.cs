using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownFood : MonoBehaviour
{
    Transform zoilForm = null;
    FoodAllocator foodAllocator;


    private void Awake()
    {
        foodAllocator = GameObject.FindFirstObjectByType<FoodAllocator>();
        zoilForm = GameObject.FindGameObjectWithTag("Zoil").transform;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, zoilForm.position) <= foodAllocator.eatRange)
        {
            foodAllocator.IncrementFoodEaten();
            Destroy(gameObject);
        }
    }
}
