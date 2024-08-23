using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using UnityEngine.SceneManagement;

public class CanvaEscScript : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject escCanvas;
    private CarController carControllerScript;
    public string scene;

  

    void Start()
    {
        mainCanvas.SetActive(true);
        escCanvas.SetActive(false);

        
       
        GameObject carObject = GameObject.FindGameObjectWithTag("Car");
        if (carObject != null)
        {
            carControllerScript = carObject.GetComponent<CarController>();
            if (carControllerScript != null)
            {
                carControllerScript.enabled = true;
            }
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (carControllerScript != null)
            {
                carControllerScript.ApplyRearBrake(0);
                carControllerScript.enabled = false;
            }

            mainCanvas.SetActive(false);
            escCanvas.SetActive(true);
        }
    }

    public void Priovos()
    {
        mainCanvas.SetActive(true);
        escCanvas.SetActive(false);

        if (carControllerScript != null)
        {
            carControllerScript.enabled = true; 
        }
    }

    public void ScenePerehid()
    {
        carControllerScript.AddCoins1();
        SceneManager.LoadScene(scene);
        
    }
}
