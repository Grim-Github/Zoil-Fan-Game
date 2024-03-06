using TMPro;
using UnityEngine;

public class StreamerController : MonoBehaviour
{
    [Header("Streaming Main")]
    public bool isStreaming = false;

    public int viewCount = 0;

    [Header("Streaming Ticks")]
    [SerializeField] private float viwerTick = 1;
    [SerializeField] private float streamerTick = 60;

    [Header("Streaming Stats")]
    public float viewCountMultipler = 1;
    public float viewCountGain = 20;
    [SerializeField] private float moneyMultipler = 0.2f;
    [SerializeField][Range(0, 100)] private float chanceToStream = 50;


    [Header("Streaming Componenets")]
    [SerializeField] private GameObject streamingSetup;
    [SerializeField]
    public string[] allStreamingCategories = {
    "Super Mario Bros. (1985)",
    "The Legend of Zelda: Ocarina of Time (1998)",
    "Tetris (1984)",
    "Pac-Man (1980)",
    "Pokémon Red and Blue (1996)",
    "Doom (1993)",
    "Super Metroid (1994)",
    "Metal Gear Solid (1998)",
    "Half-Life 2 (2004)",
    "Final Fantasy VII (1997)",
    "Counter-Strike: Global Offensive (2012)",
    "Street Fighter II (1991)",
    "Mega Man 2 (1988)",
    "Donkey Kong (1981)",
    "Sonic the Hedgehog (1991)",
    "Batman: Arkham City (2011)",
    "Super Smash Bros. Melee (2001)",
    "Star Wars: Knights of the Old Republic (2003)",
    "Super Mario Odyssey (2017)",
    "Final Fantasy VI (1994)",
    "EarthBound (1994)",
    "Deus Ex (2000)",
    "Super Mario 64 (1996)",
    "Minecraft (2011)",
    "The Elder Scrolls V: Skyrim (2011)",
    "Grand Theft Auto V (2013)",
    "Portal 2 (2011)",
    "The Witcher 3: Wild Hunt (2015)",
    "Red Dead Redemption 2 (2018)",
    "Fortnite (2017)",
    "Resident Evil 4 (2005)",
    "Mass Effect 2 (2010)",
    "Halo: Combat Evolved (2001)",
    "Diablo II (2000)",
    "World of Warcraft (2004)",
    "Civilization VI (2016)",
    "StarCraft (1998)",
    "Mega Man 2 (1988)",
    "Fire Emblem: Awakening (2012)",
    "Overwatch (2016)",
    "Super Mario Odyssey (2017)",
    "Final Fantasy VI (1994)",
    "EarthBound (1994)",
    "Uncharted 2: Among Thieves (2009)",
    "The Sims (2000)",
    "Metal Gear Solid 3: Snake Eater (2004)",
    "Shadow of the Colossus (2005)",
    "Rocket League (2015)",
    "The Legend of Zelda: A Link to the Past (1991)",
    "Deus Ex (2000)",
    "Super Mario 64 (1996)",
    "Minecraft (2011)",
    "Persona 5 (2016)",
    "God of War (2018)",
    "Team Fortress 2 (2007)",
    "Metroid Prime (2002)",
    "Journey (2012)",
    "Celeste (2018)",
    "Hollow Knight (2017)",
    "Bloodborne (2015)",
    "Cuphead (2017)",
    "Shovel Knight (2014)",
    "Undertale (2015)",
    "Stardew Valley (2016)",
    "Inside (2016)",
    "The Witness (2016)",
    "Ori and the Blind Forest (2015)",
    "Papers, Please (2013)",
    "Gone Home (2013)",
    "Limbo (2010)",
    "Braid (2008)",
    "Okami (2006)",
    "Fable (2004)",
    "Beyond Good & Evil (2003)",
    "Ico (2001)",
    "Baldur's Gate II: Shadows of Amn (2000)",
    "Grim Fandango (1998)",
    "Fallout 2 (1998)",
    "System Shock 2 (1999)",
    "Thief II: The Metal Age (2000)"
};

    [SerializeField] private TMP_Dropdown categoryDropdown;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI viwerText;
    [SerializeField] private TextMeshProUGUI streamingStatusText;

    private float remainingStreamerTick = 0;
    private Money playerMoney;
    private float remainingViwerTick = 0;
    private FoodAllocator foodAllocator;

    private void Awake()
    {
        playerMoney = FindFirstObjectByType<Money>();
        foodAllocator = GetComponent<FoodAllocator>();
        remainingStreamerTick = streamerTick;
        foreach (var category in allStreamingCategories)

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
                if (chanceToStream > 0)
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
        viwerText.text = viewCount.ToString() + " VIWERS";

        if (remainingViwerTick > 0)
        {
            remainingViwerTick -= Time.deltaTime;
        }
        else
        {
            remainingViwerTick = viwerTick;
            if (isStreaming)
            {
                viewCount += Mathf.RoundToInt(Random.Range(0, viewCountGain * viewCountMultipler));
            }
        }
    }

    private void Update()
    {

        UpdateViwerCount();

        categoryDropdown.gameObject.SetActive(!isStreaming);
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
        if (isStreaming == value)
        {
            return;
        }

        isStreaming = value;
        playerMoney.AddMoney(Mathf.RoundToInt((float)viewCount * moneyMultipler));
        viewCount = 0;

        if (isStreaming)
        {
            streamingStatusText.text = "ONLINE";
        }
        else
        {
            streamingStatusText.text = "OFFLINE";
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
