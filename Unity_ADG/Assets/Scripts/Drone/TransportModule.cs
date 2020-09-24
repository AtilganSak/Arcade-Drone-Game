using System;
using UnityEngine;

public class TransportModule : MonoBehaviour
{
    public bool enable = true;

    public Transform snapPoint;
    public LookAt indicatorArrow;

    public AudioClip receivedCargoSound;
    public AudioClip deliveredCargoSound;

    Cargo Cargo;

    public bool receivedCargo { get; private set; }
    public bool releasedCargo { get; private set; }

    public Action<Cargo> OnReceived;
    public Action<Cargo> OnDelivered;

    AudioSource audioSource;

    GameObject indicatorArrowBody;
    Transform c_Transform;
    public  Transform Transform
    {
        get
        {
            if (c_Transform == null)
                c_Transform = transform;
            return c_Transform;
        }
    }

    private void OnEnable()
    {
        if (indicatorArrow)
        {
            indicatorArrowBody = indicatorArrow.transform.GetChild(0).gameObject;
            indicatorArrowBody.SetActive(false);
        }
        c_Transform = transform;
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!enable) return;
        if (!receivedCargo)
        {
            if (other.GetComponent<Cargo>())
            {
                Cargo cargo = other.GetComponent<Cargo>();
                if (cargo.receivable)
                {
                    ReceiveCargo(cargo);
                }
            }
        }
    }
    void ReceiveCargo(Cargo cargo)
    {
        Cargo = cargo;

        Cargo.SnapTransport(snapPoint == null ? c_Transform : snapPoint);

        Cargo.Received();

        indicatorArrow.target = cargo.deliveryPlace.transform;
        indicatorArrowBody.SetActive(true);

        if (receivedCargoSound)
            audioSource.PlayOneShot(receivedCargoSound);

        if (OnReceived != null)
            OnReceived.Invoke(Cargo);

        receivedCargo = true;
        releasedCargo = false;
    }
    public void DeliverCargo(Transform dropPoint)
    {
        if (receivedCargo)
        {
            Cargo.SnapDeliveryPlace(dropPoint);

            Cargo.Delivered();

            indicatorArrowBody.SetActive(false);

            if (deliveredCargoSound)
                audioSource.PlayOneShot(deliveredCargoSound);

            if (OnDelivered != null)
                OnDelivered.Invoke(Cargo);

            Cargo = null;

            receivedCargo = false;
            releasedCargo = true;
        }
    }
}
