using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Food : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public List<Transform> wayPoints = new List<Transform>();
    [HideInInspector] public TextMeshPro nameTextMeshPro;

    [Header("Food Parameters")]
    public string foodName = "Default";
    public Color foodColor = Color.white;


    private void Awake()
    {
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("SpawnZone"))
        {
            wayPoints.Add(t.transform);
        }
        nameTextMeshPro = GetComponentInChildren<TextMeshPro>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        if (!agent.pathPending && agent.remainingDistance < 1)
        {
            agent.SetDestination(wayPoints[Random.Range(0, wayPoints.Count)].position);
        }
    }
}
