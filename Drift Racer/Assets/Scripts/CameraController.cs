using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCameraScript : MonoBehaviour
{
    public float vidstan = 7;
    public float vysota = 3;
    public float zatyshokObertannya = 0.8f;
    public float zatyshokVysoty = 0.8f;
    public float koefitsiyentZumu = 0.5f;

    private Vector3 vektorObertannya;
    private Transform avto;

    void Start()
    {
        GameObject avtoObyekt = GameObject.FindGameObjectWithTag("Car");
        if (avtoObyekt != null)
        {
            avto = avtoObyekt.transform;
        }
        else
        {
            Debug.LogError("ќб'Їкт з тегом 'Car' не знайдено!");
        }
    }

    void LateUpdate()
    {
        if (avto == null) 
        {
            Debug.LogError("јвто не було знайдено");
            return;
        }

        float bazovyiKut = vektorObertannya.y;
        float bazovaVysota = avto.position.y + vysota;

        float moyKut = transform.eulerAngles.y;
        float moyaVysota = transform.position.y;

        moyKut = Mathf.LerpAngle(moyKut, bazovyiKut, zatyshokObertannya * Time.deltaTime);
        moyaVysota = Mathf.Lerp(moyaVysota, bazovaVysota, zatyshokVysoty * Time.deltaTime);

        Quaternion potocneObertannya = Quaternion.Euler(0, moyKut, 0);

        transform.position = avto.position;
        transform.position -= potocneObertannya * Vector3.forward * vidstan;

        Vector3 temp = transform.position;

        temp.y = moyaVysota;

        transform.position = temp;

        transform.LookAt(avto);
    }

    void FixedUpdate()
    {
        if (avto == null) 
        {
            Debug.LogError("јвто не було знайдено");
            return;
        }

        Vector3 lokalnaShvydkist = avto.InverseTransformDirection(avto.GetComponent<Rigidbody>().velocity);
        if (lokalnaShvydkist.z < -0.1f) // рух задн≥м ходом
        {
            Vector3 temp = vektorObertannya;
            temp.y = avto.eulerAngles.y + 180;
            vektorObertannya = temp;
        }
        else // рух вперед
        {
            Vector3 temp = vektorObertannya;
            temp.y = avto.eulerAngles.y;
            vektorObertannya = temp;
        }
    }
}
