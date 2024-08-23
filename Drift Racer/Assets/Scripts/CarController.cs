using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CarController : MonoBehaviour
{
    [Header("Локальні значення")]
    public float horizontalInput, verticalInput;
    public float steeringAngle;
    public bool isDriveingForward;
    public float localVelocityZ; 
    public float localVelocityX;
    public float carSpeed;
    public Rigidbody carRigidbody;

    public Vector3 bodyMassCenter;

    //для дріфту
    public WheelFrictionCurve defTrenie;
    public WheelFrictionCurve castomTrenieFront;
    public WheelFrictionCurve castomTrenieRear;


    [Header("Основна характеристика авто")]
    public float maxPovorot = 30;
    public float torque = 60;
    public float brakeForce = 80;

    [Header("Времінні значення")]
    public float targetTorque = 10f;
    public float torqueChangeDuration = 2f;
    public float currentTorque = 1f; 
    public float accelerationRate = 5f; 
    public float decelerationRate = 7f;
    public float maxSpeed = 150f; 
    public int Ochki;

    [Header("Колайдери та Об'єкти")]
    public WheelCollider frontRightWheel;
    public WheelCollider frontLeftWheel;
    public WheelCollider rearRightWheel;
    public WheelCollider rearLeftWheel;

    public Transform frontRightTransform;
    public Transform frontLeftTransform;
    public Transform rearRightTransfrom;
    public Transform rearLeftTransform;

    [Header("Налаштування дрифту")]
    public bool isDrifting = false;
    public float driftThreshold = 2.5f;
    public float driftSidewaysSlip = 1.8f;
    public float driftSidewaysStiffness = 1.15f;
    public float driftRearSlip = 2.2f;
    public float driftRearStiffness = 0.95f;
    public float driftMotorTorqueMultiplier = 10f;

    [Header("Таймер очків")]
    public float scoreInterval = 1.0f;
    private float timer = 0.5f;

    [Header("Система частиць")]
    public ParticleSystem smokeParticles1;
    public ParticleSystem smokeParticles2;
    public GameObject PrefabSmoke;
    public Transform rearTransform1;
    public Transform rearTransform2;
    private float currentSmokeLifetime1 = 0f;
    private float currentSmokeLifetime2 = 0f;
    private float targetSmokeLifetime1 = 0f;
    private float targetSmokeLifetime2 = 0f;
    private Quaternion initialCarRotation;
    private int previousMaxParticles1;
    private int previousMaxParticles2;

    [Header("Позиція для респавну авто")]
    public Vector3 CarPositionRespawn = new Vector3(53, -0.6f, 96);
    public Quaternion CarRotationRespawn = new Quaternion(0, 0, 0, 0);

    private CoinsManager coinsManager;

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        isDriveingForward = localVelocityZ > 0.5f;
    }

    private void Accelerate()
    {
        float appliedTorque = torque * verticalInput;

        
        rearLeftWheel.motorTorque = appliedTorque;
        rearRightWheel.motorTorque = appliedTorque;
    }

    public void Brake()
    {
        if (verticalInput == -1 && isDriveingForward)
        {
            ApplyBrake(brakeForce);
        }
        else
        {
            ApplyBrake(0);
            if (verticalInput == 0)
            {
                MotorBrake();
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            HandBrake();
        }
        else
        {
            ApplyRearBrake(0);
        }
    }

    private void ApplyBrake(float brakeTorque)
    {
        frontLeftWheel.brakeTorque = brakeTorque;
        frontRightWheel.brakeTorque = brakeTorque;
        rearLeftWheel.brakeTorque = brakeTorque;
        rearRightWheel.brakeTorque = brakeTorque;
    }

    public void ApplyRearBrake(float brakeTorque)
    {
        rearLeftWheel.brakeTorque = brakeTorque;
        rearRightWheel.brakeTorque = brakeTorque;
    }

    public void MotorBrake()
    {
        ApplyBrake(brakeForce / 50);
    }

    public void HandBrake()
    {
        ApplyRearBrake(brakeForce);
        if (isDrifting)
        {
            AdjustFrictionForDrift();
        }
    }

    private void AdjustFrictionForDrift()
    {
        castomTrenieFront.extremumSlip = driftSidewaysSlip;
        castomTrenieFront.stiffness = driftSidewaysStiffness;
        castomTrenieRear.extremumSlip = driftRearSlip;
        castomTrenieRear.stiffness = driftRearStiffness;

        frontLeftWheel.sidewaysFriction = castomTrenieFront;
        frontRightWheel.sidewaysFriction = castomTrenieFront;
        rearLeftWheel.sidewaysFriction = castomTrenieRear;
        rearRightWheel.sidewaysFriction = castomTrenieRear;

        rearLeftWheel.motorTorque = torque * verticalInput * driftMotorTorqueMultiplier;
        rearRightWheel.motorTorque = torque * verticalInput * driftMotorTorqueMultiplier;
    }

    private void ResetFriction()
    {
        frontLeftWheel.sidewaysFriction = defTrenie;
        frontRightWheel.sidewaysFriction = defTrenie;
        rearLeftWheel.sidewaysFriction = defTrenie;
        rearRightWheel.sidewaysFriction = defTrenie;
    }

    private void Steer()
    {
        steeringAngle = maxPovorot * horizontalInput;
        frontLeftWheel.steerAngle = steeringAngle;
        frontRightWheel.steerAngle = steeringAngle;
    }

    public void Drift()
    {
        if (isDrifting)
        {
            AdjustFrictionForDrift();
        }
        else
        {
            ResetFriction();
        }
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLeftWheel, frontLeftTransform);
        UpdateWheelPose(frontRightWheel, frontRightTransform);
        UpdateWheelPose(rearLeftWheel, rearLeftTransform);
        UpdateWheelPose(rearRightWheel, rearRightTransfrom);
    }

    private void UpdateWheelPose(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos = wheelTransform.position;
        Quaternion rot = wheelTransform.rotation;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void Start()
    {
        coinsManager = FindObjectOfType<CoinsManager>();
        PrefabSmoke.SetActive(true);
        carRigidbody = GetComponent<Rigidbody>();
        defTrenie = frontLeftWheel.sidewaysFriction;
        carRigidbody.centerOfMass = bodyMassCenter;


        castomTrenieFront = frontLeftWheel.sidewaysFriction;
        castomTrenieRear = frontLeftWheel.sidewaysFriction;
        castomTrenieFront.extremumSlip = 1.39f;
        castomTrenieRear.extremumSlip = 1.8f;
        castomTrenieFront.stiffness = 1f;
        initialCarRotation = transform.rotation;
        InvokeRepeating("ChangeSmokeLifetime", 0f, 0.1f);
    }

    private void Update()
    {
        Respawn();
        carSpeed = carRigidbody.velocity.magnitude * 3.6f * 1.3f;

        if (carSpeed > maxSpeed)
        {
            carSpeed = maxSpeed;
            carRigidbody.velocity = carRigidbody.velocity.normalized * (maxSpeed / 3.6f);
        }

        localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;
        localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;

        DriftCheck();
        GetInput();
        Steer();
        Drift();
        Brake();
        Accelerate();
        UpdateWheelPoses();
        ParticleController();
    }

    private void DriftCheck()
    {
        isDrifting = Mathf.Abs(localVelocityX) > driftThreshold;
        if (isDrifting == true)
        {

            timer += Time.deltaTime;
            if (timer >= scoreInterval)
            {
                Ochki++;
                Ochki += Mathf.RoundToInt(carSpeed);
                timer = 0.0f;
            }
        }
        else
        {
            timer = 0.0f;
        }
    }

    private void ChangeSmokeLifetime()
    {
        if (isDrifting)
        {
            targetSmokeLifetime1 = 10f;
            targetSmokeLifetime2 = 10f;
        }
        else
        {
            targetSmokeLifetime1 = 0f;
            targetSmokeLifetime2 = 0f;
        }
    }

    private void ParticleController()
    {
        var main1 = smokeParticles1.main;
        var main2 = smokeParticles2.main;

        var emission1 = smokeParticles1.emission;
        var emission2 = smokeParticles2.emission;

        bool isDrifting = Mathf.Abs(localVelocityX) > driftThreshold;

        if (!isDrifting)
        {
            main1.maxParticles = 0;
            main2.maxParticles = 0;
            emission1.enabled = false;
            emission2.enabled = false;
            return;
        }

        emission1.enabled = true;
        emission2.enabled = true;

        float maxParticles1 = Mathf.Clamp(currentSmokeLifetime1 * 10, 0, 100);
        float maxParticles2 = Mathf.Clamp(currentSmokeLifetime2 * 10, 0, 100);
        main1.maxParticles = (int)maxParticles1;
        main2.maxParticles = (int)maxParticles2;

        Vector3 localVelocity = transform.InverseTransformDirection(carRigidbody.velocity);
        float angle = Mathf.Atan2(localVelocity.x, localVelocity.z) * Mathf.Rad2Deg;
        float finalAngle = angle + 180f;
        Quaternion smokeRotation = Quaternion.Euler(0f, finalAngle, 0f);

        smokeParticles1.transform.position = rearTransform1.position;
        smokeParticles2.transform.position = rearTransform2.position;

        smokeParticles1.transform.rotation = transform.rotation * smokeRotation;
        smokeParticles2.transform.rotation = transform.rotation * smokeRotation;

        main1.startRotationY = Mathf.Deg2Rad * (finalAngle);
        main2.startRotationY = Mathf.Deg2Rad * (finalAngle);

        emission1.rateOverTime = currentSmokeLifetime1 * 2;
        emission2.rateOverTime = currentSmokeLifetime2 * 2;

        if (currentSmokeLifetime1 < targetSmokeLifetime1)
        {
            currentSmokeLifetime1 += 5 * Time.deltaTime;
        }
        else if (currentSmokeLifetime1 > targetSmokeLifetime1)
        {
            currentSmokeLifetime1 -= 10 * Time.deltaTime;
        }

        if (currentSmokeLifetime2 < targetSmokeLifetime2)
        {
            currentSmokeLifetime2 += 5 * Time.deltaTime;
        }
        else if (currentSmokeLifetime2 > targetSmokeLifetime2)
        {
            currentSmokeLifetime2 -= 10 * Time.deltaTime;
        }
    }
    public void Respawn()
    {
        if (carRigidbody.position.y < -5)
        {
            carRigidbody.velocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
            transform.position = CarPositionRespawn;
            transform.rotation = CarRotationRespawn;
            Ochki = 0;
        }
    }
    public void AddCoins1()
    {
        if (coinsManager != null)
        {
            coinsManager.AddCoins(Ochki);
        }
        else
        {
            Debug.LogWarning("Функція AddCoins1() не спрацювала ");
        }
    }
}
