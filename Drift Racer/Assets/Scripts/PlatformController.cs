using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformController : MonoBehaviour
{
    public Transform Platform;
    public Collider Platform_Collider;
    public float speed = 40.0f;
    private bool rotateClockwise = true;
    public Button Button_rotation_direction;

    
    void Update()
    {


        if (rotateClockwise)
        {
            platformRight();
        }
        else
        {
            platformLeft();
        }
    }

    public void platformRight()
    {
        if (Platform != null && Platform_Collider != null)
        {
            
            Platform.Rotate(-Vector3.forward * speed * Time.deltaTime);
            Platform_Collider.transform.Rotate(-Vector3.forward * speed * Time.deltaTime);
        }
        else
        {
            
        }
    }

    public void platformLeft()
    {
        if (Platform != null && Platform_Collider != null)
        {
            
            Platform.Rotate(Vector3.forward * speed * Time.deltaTime);
            Platform_Collider.transform.Rotate(Vector3.forward * speed * Time.deltaTime);
        }
        else
        {
             Debug.LogWarning("Колайдер платформи не знайдено");
        }
    }

    public void button_rotation_direction() 
    {
        rotateClockwise = !rotateClockwise;
    }


}
