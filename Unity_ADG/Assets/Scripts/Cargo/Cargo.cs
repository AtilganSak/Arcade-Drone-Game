using System;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    public bool available = true;

    public DeliveryPlace deliveryPlace;

    public bool isCarrying { get; private set; }

    Rigidbody rigid;
    public Rigidbody Rigidbody
    {
        get
        {
            if (rigid == null)
                rigid = GetComponentInParent<Rigidbody>();

            return rigid;
        }
        set
        {
            rigid = value;
        }
    }

    Transform t;
    public Transform Transform
    {
        get
        {
            if (t == null)
                t = GetComponent<Transform>();

            return t;
        }
    }

    Collider coll;
    public Collider Collider
    {
        get
        {
            if (coll == null)
                coll = GetComponent<Collider>();

            return coll;
        }
    }

    Transform downPivot;
    Transform upPivot;

    RaycastHit rayCastHit;

    private void Start()
    {
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
