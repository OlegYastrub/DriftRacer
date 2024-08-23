using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;


public class GarazhScript : MonoBehaviour
{
    public TMP_Text CarName;
    public TMP_Text MaxSpeed;
    public TMP_Text LST;
    public TMP_Text SizeCar;
    public TMP_Text Privod;
    public TMP_Text Vaga;

    public Transform platform;
    public carInfo[] cars;
    public Vector3 spawnOffset = Vector3.up;
    public Vector3 rotationAngles = Vector3.zero;

    private int currentIndex = 0;
    private GameObject currentCar;

    public bool ProgressClear = false;

    void Start()
    {
        GameManager.Instance.garazhScript = this;
        LoadCarData();

        if (cars.Length > 0)
        {
            currentIndex = GameManager.Instance.CurrentCarIndex;
            SpawnCar();
            InfoCar();
        }
    }

    public void Update()
    {
        ClearProgress();
    }

    public void InfoCar()
    {
        if (cars.Length > 0 && currentIndex >= 0 && currentIndex < cars.Length)
        {
            carInfo currentCarInfo = cars[currentIndex];
            if (CarName != null) CarName.text = currentCarInfo.carName;
            if (MaxSpeed != null) MaxSpeed.text = currentCarInfo.maxSpeed;
            if (LST != null) LST.text = currentCarInfo.lst;
            if (SizeCar != null) SizeCar.text = currentCarInfo.sizeCar;
            if (Privod != null) Privod.text = currentCarInfo.privod;
            if (Vaga != null) Vaga.text = currentCarInfo.vaga;
        }
    }

    public void NextCar()
    {
        if (cars.Length > 1)
        {
            DestroyCurrentCar();
            FindNextVisibleCar(1);
            SpawnCar();
            InfoCar();
        }
    }

    public void PreviousCar()
    {
        if (cars.Length > 1)
        {
            DestroyCurrentCar();
            FindNextVisibleCar(-1);
            SpawnCar();
            InfoCar();
        }
    }

    private void DestroyCurrentCar()
    {
        if (currentCar != null)
        {
            Destroy(currentCar);
        }
    }

    void SpawnCar()
    {
        if (cars.Length > 0 && currentIndex >= 0 && currentIndex < cars.Length)
        {
            Vector3 spawnPosition = platform.position + spawnOffset;
            Quaternion spawnRotation = Quaternion.Euler(rotationAngles);
            currentCar = Instantiate(cars[currentIndex].carPrefab, spawnPosition, spawnRotation);
            currentCar.transform.parent = platform;
            Debug.Log($"Авто з індексом {currentIndex} заспанилось на платформі з позицієй {spawnPosition} та направленням{spawnRotation}");
        }
        else
        {
            Debug.LogError("Авто по індексу не знайдено");
        }
    }

    public void FindNextVisibleCar(int direction)
    {
        if (cars.Length == 0) return;

        int startingIndex = currentIndex;
        do
        {
            currentIndex = (currentIndex + direction + cars.Length) % cars.Length;
        } while (!cars[currentIndex].isBuy && currentIndex != startingIndex);

        if (!cars[currentIndex].isBuy)
        {
            currentIndex = startingIndex;
        }

        GameManager.Instance.CurrentCarIndex = currentIndex;
    }


    public int isCarBuy(int index)
    {
        if (index >= 0 && index < cars.Length)
        {
            return 1;
        }
        else
        {
            Debug.LogError($"Індекс {index} не знайдено");
            return 0;
        }
    }

    public void carBuyId(int index)
    {
        if (index >= 0 && index < cars.Length)
        {
            cars[index].isBuy = true;
            SaveCarData(index);
            Debug.Log($"Авто з індексом {index} куплено = {cars[index].isBuy}");
        }
        else
        {
            Debug.LogError($"Авто з індексом {index} не знайдено");
        }
    }

    private void SaveCarData(int index)
    {
        PlayerPrefs.SetInt($"Car_{index}_IsBought", cars[index].isBuy ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"Авто з індексом {index} збережено");
    }


    private void LoadCarData()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            if (i == 0 && !PlayerPrefs.HasKey($"Car_{i}_IsBought"))
            {

                cars[i].isBuy = true;
                SaveCarData(i);
            }
            else
            {
                cars[i].isBuy = PlayerPrefs.GetInt($"Car_{i}_IsBought", 0) == 1;
            }
        }
    }

    private void ClearProgress()
    {
        if (ProgressClear == true)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Функція очищення спрацювала");
            ProgressClear = false;
        }
    }
}
