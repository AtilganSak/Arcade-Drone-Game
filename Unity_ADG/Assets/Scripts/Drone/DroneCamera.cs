using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCamera : MonoBehaviour
{
    [SerializeField] DroneController drone;

    public int smooting;

    public Vector3 offset;

    public float angle;

    Transform myTransform;

    void Start()
    {
        offset = transform.position - drone.myTransform.position;
    }
    private void OnEnable()
    {
        myTransform = GetComponent<Transform>();
    }
    void LateUpdate()
    {
        myTransform.LookAt(drone.myTransform);

        myTransform.localPosition = Vector3.Slerp(myTransform.localPosition, drone.myTransform.TransformPoint(offset), Time.fixedDeltaTime * smooting);
    }
}
