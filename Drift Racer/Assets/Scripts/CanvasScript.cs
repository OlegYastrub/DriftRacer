using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{
    public bool ActiveMenu = false;
    public GameObject[] button;
    public GameObject panel;
    public Camera mainCamera;
    public Vector3 cameraPosition1 = new Vector3(-2.79f, -3.5f, -4.14f); //дєфолд позиція
    public Vector3 cameraPosition2 = new Vector3(-2.79f, -3.5f, -2.10f); //кастомний перехід
    public float ChasPerehidu = 1.0f; 
   

    
    void Start()
    {
        mainCamera.transform.position = cameraPosition1; // встановлення початкової позиції
    }

   
    void Update()
    {
        
    }

    public void ActiveButtonOff()
    {
        ActiveMenu = !ActiveMenu;

        if (ActiveMenu == true)
        {
            foreach (GameObject btn in button)
            {
                btn.SetActive(false);
            }
            panel.SetActive(true);
            StartCoroutine(MoveCamera(cameraPosition2));
            
        }
        else
        {
            foreach (GameObject btn in button)
            {
                btn.SetActive(true);
            }
            panel.SetActive(false);
            StartCoroutine(MoveCamera(cameraPosition1));
        }
    }

    private IEnumerator MoveCamera(Vector3 targetPosition)
    {
        Vector3 startPosition = mainCamera.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < ChasPerehidu)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / ChasPerehidu;
            t = t * t * (3f - 2f * t); // для плавності перехіду
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
    }
}
