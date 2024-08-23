using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Car
{
    public GameObject carPrefab;
}

public class CarUI : MonoBehaviour
{
    public Car[] cars;
    public Vector3 spawnOffset;
    public Quaternion rotation;
    private GameObject currentCar;
    private CarController carController;
    private TextMeshProUGUI carSpeedText;

    private TextMeshProUGUI ochkiUI;
    private TextMeshProUGUI Coins;




    private void Start()
    {

        Coins = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
        Coins.enabled = true;
        

        SpawnCarID(GameManager.Instance.CurrentCarIndex);
        carController = FindObjectOfType<CarController>();

        carSpeedText = GameObject.Find("SpeedTextTMP").GetComponent<TextMeshProUGUI>();
        ochkiUI = GameObject.Find("OchkiTextTMP").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        UpdateCarSpeedUI();

        UpdateOchkiUI();
    }

    private void UpdateCarSpeedUI()
    {
        if (carSpeedText != null)
        {
            float absoluteCarSpeed = Mathf.Abs(carController.carSpeed);
            carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
        }
    }



    private void UpdateOchkiUI()
    {
        if (ochkiUI != null)
        {
            ochkiUI.text = carController.Ochki.ToString("F0");

        }
    }

    public void SpawnCarID(int currentIndex)
    {
        if (cars.Length > 0 && currentIndex >= 0 && currentIndex < cars.Length)
        {
            Vector3 spawnPosition = spawnOffset;
            Quaternion spawnRotation = rotation;
            currentCar = Instantiate(cars[currentIndex].carPrefab, spawnPosition, spawnRotation);
            Debug.Log($"Авто з індексом {currentIndex} заспавнилось, на позиції {spawnPosition} угол поворота {spawnRotation}");
        }
        else
        {
            Debug.LogError("Індекс для спавну авто не знайден");
        }
    }


}
