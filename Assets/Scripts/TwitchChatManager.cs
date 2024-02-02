using Lexone.UnityTwitchChat;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TwitchChatManager : MonoBehaviour
{
    [SerializeField] private GameObject foodItems;

    [SerializeField] private List<string> spawnedNames = new List<string>();
    [SerializeField] private List<Chatter> waitList = new List<Chatter>();

    [SerializeField] private GameObject[] spawnZones;

    private FoodAllocator foodAllocator;

    [SerializeField][Range(0, 100)] public float chanceToSpawn = 1;
    [SerializeField] private bool dynamicSpawnChance = false;


    private void Start()
    {
        IRC.Instance.OnChatMessage += OnChatMessage;
    }

    private void OnChatMessage(Chatter chatter)
    {
        foodAllocator = GameObject.FindObjectOfType<FoodAllocator>();
        spawnZones = GameObject.FindGameObjectsWithTag("SpawnZone");
        if (spawnedNames.Contains(chatter.tags.displayName))
        {
            return;
        }

        int rng = Random.Range(0, 100);
        Debug.Log($"<color=#fef83e><b>[CHAT LISTENER]</b></color> New chat message from {chatter.tags.displayName} (" + rng + "/" + chanceToSpawn + ")");


        if (chanceToSpawn > rng)
        {
            GameObject food = Instantiate(foodItems, spawnZones[Random.Range(0, spawnZones.Length)].transform.position, Quaternion.identity);
            food.GetComponent<Food>().nameTextMeshPro.text = chatter.tags.displayName;
            food.GetComponent<Food>().nameTextMeshPro.color = chatter.GetNameColor();
            food.GetComponent<Food>().foodName = chatter.tags.displayName;
            food.GetComponent<Food>().foodColor = chatter.GetNameColor();
            food.gameObject.name = chatter.tags.displayName;
            spawnedNames.Add(chatter.tags.displayName);
            foodAllocator.UpdateActiveText(spawnedNames.Count);

            for (int i = 0; i < chatter.tags.badges.Length; i++)
            {
                food.GetComponentInChildren<Food>().agent.speed++;
            }

            if (dynamicSpawnChance)
            {
                chanceToSpawn -= 1;
            }
        }
        else
        {
            if(!waitList.Contains(chatter))
            {
                waitList.Add(chatter);
            }

            if(dynamicSpawnChance)
            {
                chanceToSpawn += 1;
            }
        }
    }
}