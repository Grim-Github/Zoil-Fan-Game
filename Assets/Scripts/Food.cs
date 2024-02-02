using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Food : MonoBehaviour
{
    public NavMeshAgent agent;
    public List<Transform> waypoints = new List<Transform>();
    public TextMeshPro nameTextMeshPro;
    public string foodName = "Default";
    private FoodAllocator foodAllocator;
    public Color foodColor = Color.white;

    private void Awake()
    {
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("SpawnZone"))
        {
            waypoints.Add(t.transform);
        }
        nameTextMeshPro = GetComponentInChildren<TextMeshPro>();

        agent = GetComponent<NavMeshAgent>();
        foodAllocator = GameObject.FindAnyObjectByType<FoodAllocator>();
        agent.speed += Random.Range(0, foodAllocator.foodEaten/2);
    }

    private void Update()
    {
        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        if (!agent.pathPending && agent.remainingDistance < 1)
        {
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].position);
        }
    }
}
