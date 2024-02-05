using System.Collections;
using TMPro;
using UnityEngine;

public class StreamerController : MonoBehaviour
{
    public bool isStreaming = false;
    public int viewCount = 0;
    private FoodAllocator foodAllocator;
    [SerializeField] private GameObject streamingSetup;
    [SerializeField] private float viwerTick = 1;
    [SerializeField] private float streamerTick = 60;
    [SerializeField][Range(0, 100)] private float chanceToStream = 50;
    [SerializeField] public string[] streamingCategories;
    [SerializeField] private TMP_Dropdown categoryDropdown;
    [SerializeField] private TextMeshProUGUI categoryText; [SerializeField] private TextMeshProUGUI viwerText;
    [SerializeField] private TextMeshProUGUI streamingStatusText;
    private float remainingStreamerTick = 0;
    private Money playerMoney;
    private float remainingViwerTick = 0;

    private void Awake()
    {
        playerMoney = FindFirstObjectByType<Money>();
        foodAllocator = GetComponent<FoodAllocator>();
        remainingStreamerTick = streamerTick;
        foreach (var category in streamingCategories)

        {
            categoryDropdown.options.Add(new TMP_Dropdown.OptionData(category));
        }
    }

    private void UpdateStreamStatus()
    {
        if (remainingStreamerTick > 0)
        {
            remainingStreamerTick -= Time.deltaTime;
        }
        else
        {
            if (Random.Range(0, 100) < chanceToStream)
            {
                ToggleStream(true);
                if(chanceToStream >0)
                {
                    chanceToStream -= foodAllocator.hungerRate;
                }
                categoryText.text = categoryDropdown.options[categoryDropdown.value].text;
            }
            else
            {
                ToggleStream(false);
            }

            remainingStreamerTick = streamerTick;
        }
    }
    
    private void UpdateViwerCount()
    {
        viwerText.text = viewCount.ToString();

        if (remainingViwerTick > 0)
        {
            remainingViwerTick -= Time.deltaTime;
        }
        else
        {
            remainingViwerTick = viwerTick;
            if(isStreaming)
            {
                viewCount += Random.Range(0, 20);
            }
        }
    }

    private void Update()
    {

        UpdateViwerCount();

        categoryDropdown.gameObject.SetActive(!isStreaming) ;
        categoryText.gameObject.SetActive(isStreaming);

        UpdateStreamStatus();

        if (isStreaming)
        {
            Streaming();
        }
        else
        {
            Offline();
        }
    }

    public void ToggleStream(bool value)
    {
        if(isStreaming == value)
        {
            return;
        }

        isStreaming = value;
        playerMoney.AddMoney(viewCount/5);
        viewCount = 0;

        if(isStreaming)
        {
            streamingStatusText.text = "Zoil is currently streaming";
        }
        else
        {
            streamingStatusText.text = "Zoil is currently not streaming"; 
        }
    }

    private void Offline()
    {
        foodAllocator.canEat = true;
        streamingSetup.SetActive(false);
    }

    private void Streaming()
    {
        foodAllocator.canEat = false;
        streamingSetup.SetActive(true);
    }
}
