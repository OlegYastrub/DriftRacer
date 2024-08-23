using UnityEngine;
using TMPro;

public class CoinsManager : MonoBehaviour
{
    public static CoinsManager Instance { get; private set; }

    public int coins = 0;
    public TextMeshProUGUI coinsText;
    private AutosalonScript autosalonScript;
    private string coinsKey = "Coins";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
        if (PlayerPrefs.HasKey(coinsKey))
        {
            coins = PlayerPrefs.GetInt(coinsKey);
        }

        coinsText = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
        UpdateCoinsUI();
    }

    public void Update()
    {
        if (coinsText == null)
        {
            coinsText = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
            UpdateCoinsUI();
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinsUI();
        PlayerPrefs.SetInt(coinsKey, coins);
    }

    public bool CanAfford(int price)
    {
        return coins >= price;
    }

    public bool BuyCar(int carPrice)
    {
        if (CanAfford(carPrice))
        {
            coins -= carPrice;
            UpdateCoinsUI();
            PlayerPrefs.SetInt(coinsKey, coins);
            return true;
        }
        else
        {
            Debug.Log("недостатньо коштів");
            return false;
        }
    }

    public void UpdateCoinsUI()
    {
        coinsText.text = coins.ToString();
        if (autosalonScript != null)
        {
            autosalonScript.CoinsSumL(coins);
        }
    }
}
