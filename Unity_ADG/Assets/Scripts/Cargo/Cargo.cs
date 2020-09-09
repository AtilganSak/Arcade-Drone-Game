using System;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    public bool available = true;

    public DeliveryPlace deliveryPlace;

    public bool isCarrying { get; private set; }

    public Rigidbody Rigidbody { get; private set; }
    public Transform Transform { get; private set; }
    public Collider Collider { get; private set; }
    public GameObject GameObject { get; private set; }
    public Transform downPivot { get; private set; }
    public Transform upPivot { get; private set; }
    public Target indicator { get; private set; }

    RaycastHit rayCastHit;

    private void OnEnable()
    {
        Transform = transform;
        GameObject = gameObject;
        Rigidbody = GetComponentInParent<Rigidbody>();
        Collider = GetComponent<Collider>();
        indicator = GetComponent<Target>();

        upPivot = Transform.root;
        downPivot = upPivot.GetChild(0);
    }

    public void Received()
    {
        isCarrying = true;

        deliveryPlace.Activate();
    }
    public void Delivered()
    {
        isCarrying = false;

        deliveryPlace.DeliveredCargo(this);

        available = false;
    }
    public void SnapTransport(Transform snapPoint)
    {
        upPivot.position = snapPoint.position;
        upPivot.rotation = snapPoint.rotation;

        downPivot.SetParent(snapPoint);
    }
    public void SnapDeliveryPlace(Transform snapPoint)
    {
        downPivot.SetParent(null);

        downPivot.position = snapPoint.position;
        downPivot.rotation = snapPoint.rotation;

        FixTransformBySurface();

        downPivot.SetParent(snapPoint);
    }
    void FixTransformBySurface()
    {
        if (Physics.Raycast(Transform.position, -Transform.up, out rayCastHit, 100))
        {
            downPivot.position = rayCastHit.point;
            downPivot.rotation = Quaternion.FromToRotation(downPivot.up, rayCastHit.normal) * downPivot.rotation;
        }
    }
}
