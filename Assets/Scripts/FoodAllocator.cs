using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class FoodAllocator : MonoBehaviour
{
    public bool canEat = true;
    public int foodEaten = 0;
    public float hungerRate = 2;
    public float hunger = 0;
    private Transform playerPosition;
    [SerializeField] private TextMeshProUGUI hungerText;
    [SerializeField] private TextMeshProUGUI eatenText;
    [SerializeField] private TextMeshProUGUI hungerRateText;
    [SerializeField] private TextMeshProUGUI spawnRateText;
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private TextMeshProUGUI spawnedText;
    [SerializeField] private GameObject eatStatsCanvas;
    [SerializeField] private ParticleSystem foodEatParticles;
    private PlayerInventory inventory;
    private TwitchChatManager twitchChatManager;
    private StreamerController streamerController;
    public float eatRange = 3f;

    private void Awake()
    {
        inventory = GameObject.FindObjectOfType<PlayerInventory>();
        streamerController = GameObject.FindObjectOfType<StreamerController>();
        twitchChatManager = GameObject.FindObjectOfType<TwitchChatManager>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        UpdateHungerText();
        hungerSlider.maxValue = 100;
        UpdateActiveText(0);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, eatRange);
    }

    public void LoseGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateHungerText()
    {
        hungerText.text = Mathf.RoundToInt(hunger).ToString() + " /100";
        if (streamerController.isStreaming)
        {
            hungerRateText.text = Math.Round(hungerRate/2, 2) + " HUNGER";
        }
        else
        {
            hungerRateText.text = Math.Round(hungerRate, 2) + " HUNGER";
        }
        spawnRateText.text = twitchChatManager.chanceToSpawn.ToString() + " % HAMBUGER SPREAD";
        eatenText.text = "ZOIL ATE " + foodEaten + " BUGERS";
    }

    public void UpdateActiveText(int amount)
    {
        spawnedText.text = "SPAWNED BURGERS: " + amount;
    }

    public void IncrementFoodEaten()
    {
        foodEaten++;
        hunger -= 10;
        foodEatParticles.Play();
    }

    public void WorkerApproachEat()
    {
        IncrementFoodEaten();
    }

    private void PlayerApproachEat()
    {


        if (Vector3.Distance(transform.position, playerPosition.position) < eatRange)
        {
            eatStatsCanvas.SetActive(true);

            if (canEat == false)
            {
                return;
            }

            if (inventory.amountOfItems > 0)
            {
                for (int i = 0; i < inventory.amountOfItems; i++)
                {
                    inventory.DispatchOneItem();
                    IncrementFoodEaten();
                }
            }
        }
        else
        {
            eatStatsCanvas.SetActive(false);
        }
    }

    private void UpdateHungerSlider()
    {
        hungerSlider.value = hunger;
    }

    private void Update()
    {
        if (streamerController.isStreaming)
        {
            hunger += Time.deltaTime * hungerRate/2;
        }
        else
        {
            hunger += Time.deltaTime * hungerRate;
        }


        hungerRate += .02f * Time.deltaTime;
        if (hunger >= 100)
        {
            LoseGame();
        }

        UpdateHungerText();
        UpdateHungerSlider();
        PlayerApproachEat();
    }


}
