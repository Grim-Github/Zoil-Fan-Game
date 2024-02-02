using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    public NavMeshAgent agent;
    public float maxSearchRadius = 10f;
    public float pickupRadius = 2;

    public Transform[] agentInventory;
    public List<Transform> waypoints = new List<Transform>();
    private bool fullInventory = false;
    private Vector3 startingPos = Vector3.zero;


    public enum Moods
    {
        Happy,
        Sad,
        Angry,
    }

    public Moods currentMood = Moods.Happy;

    private void Awake()
    {
        startingPos = transform.position;
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("SpawnZone"))
        {
            waypoints.Add(t.transform);
        }
    }

    private void Update()
    {
        if (!fullInventory)
        {
            WalkToNearComponent();
        }
        else
        {
            WalkToFoodAllocator(GameObject.FindAnyObjectByType<FoodAllocator>());
        }
    }

    private void SearchMode()
    {
        if (!agent.pathPending && agent.remainingDistance < 1)
        {
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].position);
        }
    }

    private void WalkToFoodAllocator(FoodAllocator foodAllocator)
    {
        if (Vector3.Distance(transform.position, foodAllocator.transform.position) < foodAllocator.eatRange)
        {
            fullInventory = false;

            for (int i = 0; i < agentInventory.Length; i++)
            {
                if (agentInventory[i].gameObject.activeSelf == true)
                {
                    agentInventory[i].gameObject.SetActive(false);
                    foodAllocator.WorkerApproachEat();
                }
            }
        }
        agent.SetDestination(foodAllocator.transform.position);
    }

    private void OnReachComponent(Transform target)
    {
        if (Vector3.Distance(transform.position, target.transform.position) < pickupRadius)
        {
            agent.SetDestination(transform.position);

            for (int i = 0; i < agentInventory.Length; i++)
            {
                if (agentInventory[i].gameObject.activeSelf == false)
                {
                    agentInventory[i].gameObject.SetActive(true);
                    agentInventory[i].GetComponentInChildren<TextMeshPro>().text = target.transform.GetComponent<Food>().foodName;
                    agentInventory[i].GetComponentInChildren<TextMeshPro>().color = target.transform.GetComponent<Food>().foodColor;
                    break;
                }
            }

            fullInventory = AreAllGameObjectsActive(agentInventory);

            Destroy(target.gameObject);
        }
    }

    bool AreAllGameObjectsActive(Transform[] gameObjects)
    {
        foreach (Transform obj in gameObjects)
        {
            if (!obj.gameObject.activeSelf)
            {
                return false; // If any object is inactive, return false
            }
        }

        return true; // If all objects are active, return true
    }


    private void WalkToNearComponent()
    {
        Food nearestFoodComponent = GetNearestFoodComponent();

        if (agent != null)
        {
            if (nearestFoodComponent != null)
            {
                agent.SetDestination(GetNearestFoodComponent().transform.position);
                OnReachComponent(nearestFoodComponent.transform);
            }
            else
            {
                SearchMode();
            }
        }
    }

    private void CheckForNearComponent()
    {
        Food nearestFoodComponent = GetNearestFoodComponent();

        if (nearestFoodComponent != null)
        {
            Debug.Log("Nearest food component found on object: " + nearestFoodComponent.gameObject.name);
        }
        else
        {
            Debug.Log("No object with Food component found within the search radius.");
        }
    }

    private Food GetNearestFoodComponent()
    {
        Food[] allFoodComponents = FindObjectsOfType<Food>();

        Food nearestFoodComponent = null;
        float nearestDistance = float.MaxValue;

        foreach (Food foodComponent in allFoodComponents)
        {
            float distance = Vector3.Distance(transform.position, foodComponent.transform.position);

            if (distance < nearestDistance)
            {
                nearestFoodComponent = foodComponent;
                nearestDistance = distance;
            }
        }

        if (nearestDistance <= maxSearchRadius)
        {
            return nearestFoodComponent;
        }
        else
        {
            return null;
        }
    }
}
