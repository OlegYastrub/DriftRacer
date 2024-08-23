using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class carInfo
{
    public GameObject carPrefab;
    public string carName;
    public string maxSpeed;
    public string lst;
    public string sizeCar;
    public string privod;
    public string vaga;
    public string sumCar;
    public bool isBuy;
}

public class AutosalonScript : MonoBehaviour
{
    public TMP_Text CarName;
    public TMP_Text MaxSpeed;
    public TMP_Text LST;
    public TMP_Text SizeCar;
    public TMP_Text Privod;
    public TMP_Text Vaga;
    public TMP_Text SumCar;
    public Button isBuyButton;

    public Transform platform;
    public carInfo[] cars;
    public Vector3 spawnOffset = Vector3.up;
    public int currentIndex = 0;
    private GameObject currentCar;
    private CoinsManager coinsManager;
    public int Coins;
    public GameObject MessagePanel;
    public TMP_Text MessegeText;
    public Button ButtonOkMessage;

    

    void Start()
    {
        PlayerPrefs.SetInt("Car_" + 0 + "_IsBuy", 1);
        coinsManager = FindObjectOfType<CoinsManager>();
        LoadCarData();

        coinsManager.UpdateCoinsUI();
        SpawnCar();
        InfoCar();
    }
    private void Update()
    {
       
        carInfo currentCarInfo = cars[currentIndex];
        if (currentCarInfo.isBuy == true)
        {
            isBuyButton.interactable = false;
        }
        else
        {
            isBuyButton.interactable = true;
        }
    }
    private void LoadCarData()
    {
        

        for (int i = 0; i < cars.Length; i++)
        {
            int isBuyValue = PlayerPrefs.GetInt("Car_" + i + "_IsBuy", 0);
            cars[i].isBuy = isBuyValue == 1 ? true : false;
        }
    }
    public void InfoCar()
    {
        if (cars.Length > 0)
        {
            carInfo currentCarInfo = cars[currentIndex];
            CarName.text = currentCarInfo.carName;
            MaxSpeed.text = currentCarInfo.maxSpeed;
            LST.text = currentCarInfo.lst;
            SizeCar.text = currentCarInfo.sizeCar;
            Privod.text = currentCarInfo.privod;
            Vaga.text = currentCarInfo.vaga;
            SumCar.text = currentCarInfo.sumCar;
        }
    }

    public void NextCar()
    {
        if (currentCar != null)
        {
            Destroy(currentCar);
        }
        currentIndex = (currentIndex + 1) % cars.Length;
        SpawnCar();
        InfoCar();
    }

    public void PreviousCar()
    {
        if (currentCar != null)
        {
            Destroy(currentCar);
        }
        currentIndex = (currentIndex - 1 + cars.Length) % cars.Length;
        SpawnCar();
        InfoCar();
    }

    void SpawnCar()
    {
        if (cars.Length > 0)
        {
            Vector3 spawnPosition = platform.position + spawnOffset;
            currentCar = Instantiate(cars[currentIndex].carPrefab, spawnPosition, Quaternion.identity);
            currentCar.transform.parent = platform;
        }
    }

    public void BuyCar()
    {
        carInfo currentCarInfo = cars[currentIndex];
        int carPrice;

        if (int.TryParse(currentCarInfo.sumCar, out carPrice))
        {
            if (coinsManager.BuyCar(carPrice))
            {
                GameManager.Instance.CurrentCarIndex = currentIndex;
                GameManager.Instance.garazhScript.carBuyId(currentIndex);
                currentCarInfo.isBuy = true;
                PlayerPrefs.SetInt("Car_" + currentIndex + "_IsBuy", 1);
                MessageActive();
                MessegeText.text =  $"Ви купили авто {currentCarInfo.carName}";
            }
            else
            {
                MessageActive();
                MessegeText.text = "Не вистачає грошей для покупки данного авто";
                Debug.Log("Не хватает денег для покупки авто");
            }
        }
        else
        {
            Debug.LogError("Конверт int не спрацював");
        }
    }

    public void CoinsSumL(int CoinsSum) 
    {
        Coins = CoinsSum;
    }
    public void MessageActive()
    {
        MessagePanel.SetActive(true);
        MessegeText = GameObject.Find("MessageText").GetComponent<TextMeshProUGUI>();
        ButtonOkMessage = GameObject.Find("ButtonOkMessage").GetComponent<Button>();
    }
    public void MesageUnActive()
    {
        MessagePanel.SetActive(false);
    }

}
