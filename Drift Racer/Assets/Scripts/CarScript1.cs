using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
using TMPro;
using UnityEditor;


public class CarScript1 : MonoBehaviour
{
    [Header("Головне")]
    public GameObject Car;

    [Header("Світло фар")]
    public Material objectMaterial;
    public Material BackFonari;
    public Light[] Fari;
    public bool isEmissionOn = false;

    [Header("Поворотники")]
    public Material BlinkMaterial;
    public bool isBlinking = false;
    public bool isEmergencyBlinking = false;
    public bool isDopbool = false;

    private Coroutine blinkCoroutine;
    private Coroutine emergencyBlinkCoroutine;

    void Start()
    {

    }

    private void Update()
    {
        Light();
        Blink();

    }

    private void SetEmission(bool isEmissionEnabled)
    {
        if (objectMaterial != null)
        {
            if (isEmissionEnabled)
            {
                objectMaterial.EnableKeyword("_EMISSION");
               
            }
            else
            {
                objectMaterial.DisableKeyword("_EMISSION");
                
            }
        }
    }

    public void Light()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            isEmissionOn = !isEmissionOn;
            SetEmission(isEmissionOn);

            foreach (Light light in Fari)
            {
                light.enabled = isEmissionOn;
            }
        }
    }

    private IEnumerator BlinkCoroutine()
    {
        while (true)
        {
            isBlinking = !isBlinking;
            SetBlink(isBlinking);
            yield return new WaitForSeconds(1f);
        }
    }

    private void SetBlink(bool isEmissionEnabled)
    {
        if (isEmissionEnabled)
        {
            BlinkMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            BlinkMaterial.DisableKeyword("_EMISSION");
        }
    }

    public void Blink()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isDopbool = !isDopbool;
            if (isDopbool == true)
            {
                if (blinkCoroutine != null)
                {
                    StopCoroutine(blinkCoroutine);
                }
                blinkCoroutine = StartCoroutine(BlinkCoroutine());
            }
            else
            {
                if (blinkCoroutine != null)
                {
                    StopCoroutine(blinkCoroutine);
                }
                isBlinking = false;
                SetBlink(isBlinking);
            }
        }
    }

    private IEnumerator EmergencyBlinkCoroutine()
    {
        while (true)
        {
            isEmergencyBlinking = !isEmergencyBlinking;
            SetBlink(isEmergencyBlinking);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void EmergencyBlink()
    {
        if (emergencyBlinkCoroutine != null)
        {
            StopCoroutine(emergencyBlinkCoroutine);
        }
        emergencyBlinkCoroutine = StartCoroutine(EmergencyBlinkCoroutine());
    }


}

