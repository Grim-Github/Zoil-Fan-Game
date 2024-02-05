using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    [Header("Worker Components")]
    public NavMeshAgent agent;
    public Transform[] agentInventory;

    [Header("Worker Stats")]
    public float workerXP = 0;
    public float maxSearchRadius = 10f;
    public float pickupRadius = 2;


    [HideInInspector] public List<Transform> waypoints = new List<Transform>();
    [HideInInspector] public List<Transform> trashBins = new List<Transform>();
    private bool fullInventory = false;

    private Vector3 startingPos = Vector3.zero;
    private float startingSpeed;
    private FoodAllocator foodAllocator;

    public enum Moods
    {
        Happy,
        Bored,
        HateWatching,
    }

    [Header("Worker Moods")]
    private float moodCheckTimer = 0;
    public float moodSwing = 120;



    public Moods currentMood = Moods.Happy;

    private void Awake()
    {
        startingPos = transform.position;
        foodAllocator = GameObject.FindAnyObjectByType<FoodAllocator>();
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("SpawnZone"))
        {
            waypoints.Add(t.transform);
        }

        foreach (GameObject t in GameObject.FindGameObjectsWithTag("TrashBin"))
        {
            trashBins.Add(t.transform);
        }

        startingSpeed = agent.speed;
        moodCheckTimer = moodSwing;
    }

    private void MoodChecks()
    {
        if (moodCheckTimer > 0)
        {
            moodCheckTimer -= Time.deltaTime;
        }
        else
        {
            currentMood = (Moods)Random.Range(0, System.Enum.GetValues(typeof(Moods)).Length);
            moodCheckTimer = moodSwing;
        }

        switch (currentMood)
        {
            case Moods.Happy:
                agent.speed = startingSpeed;
                break;
            case Moods.Bored:
                agent.speed = startingSpeed / 2;
                break;
            case Moods.HateWatching:
                Debug.Log("ANGRY");
                break;
        }
    }

    private void Update()
    {
        MoodChecks();

        if (!fullInventory)
        {
            WalkToNearComponent();
        }
        else
        {
            if (currentMood == Moods.HateWatching)
            {
                WalkToTrashBin(GetNearestTrashBin());
            }
            else
            {
                if(foodAllocator.canEat == true)
                {
                    WalkToFoodAllocator(foodAllocator);
                }
                else
                {
                    agent.SetDestination(startingPos);
                }

            }
        }
    }

    private void SearchMode()
    {
        if (!agent.pathPending && agent.remainingDistance < 1)
        {
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].position);
        }
    }

    private void WalkToTrashBin(Transform trashbin)
    {
        if (Vector3.Distance(transform.position, trashbin.position) <= pickupRadius)
        {
            fullInventory = false;
            for (int i = 0; i < agentInventory.Length; i++)
            {
                if (agentInventory[i].gameObject.activeSelf == true)
                {
                    agentInventory[i].gameObject.SetActive(false);
                }
            }
        }
        agent.SetDestination(trashbin.transform.position);
    }

    private void WalkToFoodAllocator(FoodAllocator foodAllocator)
    {
        if (foodAllocator.canEat == false)
        {
            {
                return;
            }
        }

        if (Vector3.Distance(transform.position, foodAllocator.transform.position) < foodAllocator.eatRange)
        {
            fullInventory = false;

            for (int i = 0; i < agentInventory.Length; i++)
            {
                if (agentInventory[i].gameObject.activeSelf == true)
                {
                    agentInventory[i].gameObject.SetActive(false);
                    workerXP += 10;
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


    private Transform GetNearestTrashBin()
    {
        Transform[] allFoodComponents = trashBins.ToArray();

        Transform nearestFoodComponent = null;
        float nearestDistance = float.MaxValue;

        foreach (Transform foodComponent in allFoodComponents)
        {
            float distance = Vector3.Distance(transform.position, foodComponent.transform.position);

            if (distance < nearestDistance)
            {
                nearestFoodComponent = foodComponent;
                nearestDistance = distance;
            }
        }

        return nearestFoodComponent;
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
