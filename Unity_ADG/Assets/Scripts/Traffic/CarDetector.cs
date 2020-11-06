using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDetector : MonoBehaviour
{
    public string carTag = "Car";

    public bool detectingVehicle;

    public Action<bool> onDetecting;

    public Transform detectedCar { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(carTag))
        {
            detectingVehicle = true;

            detectedCar = other.transform;

            if(onDetecting != null)
            {
                onDetecting.Invoke(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(carTag))
        {
            detectingVehicle = false;

            detectedCar = null;

            if (onDetecting != null)
            {
                onDetecting.Invoke(false);
            }
        }
    }
}
