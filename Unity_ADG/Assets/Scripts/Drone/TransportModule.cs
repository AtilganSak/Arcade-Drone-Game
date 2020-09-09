using System;
using UnityEngine;

public class TransportModule : MonoBehaviour
{
    public bool enable = true;

    public Transform snapPoint;

    public AudioClip receivedCargoSound;
    public AudioClip deliveredCargoSound;

    Cargo Cargo;

    public bool receivedCargo { get; private set; }
    public bool releasedCargo { get; private set; }

    public Action<Cargo> OnReceived;
    public Action<Cargo> OnDelivered;

    AudioSource audioSource;

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
                if (cargo.available)
                {
                    ReceiveCargo(cargo);
                }
            }
        }
    }
    void ReceiveCargo(Cargo cargo)
    {
        Cargo = cargo;

        Cargo.Rigidbody.angularVelocity = Vector3.zero;
        Cargo.Rigidbody.velocity = Vector3.zero;

        Destroy(Cargo.Rigidbody);

        Cargo.SnapTransport(snapPoint == null ? c_Transform : snapPoint);

        Cargo.Received();

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
