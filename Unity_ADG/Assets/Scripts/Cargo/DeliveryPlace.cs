using UnityEngine;

public class DeliveryPlace : MonoBehaviour
{
    public Transform dropPoint;

    public bool cargoIsHere { get; private set; }

    public Cargo cargo { get; private set; }

    bool isActive;

    Transform c_Transform;
    Collider collider;

    private void OnEnable()
    {
        c_Transform = transform;
        collider = GetComponent<Collider>();
    }
    private void Start()
    {
        Deactivate();
    }
    public void Activate()
    {
        collider.enabled = true;

        isActive = true;
    }
    public void Deactivate()
    {
        collider.enabled = false;

        isActive = false;
    }
    public void DeliveredCargo(Cargo cargo)
    {
        this.cargo = cargo;
        cargoIsHere = true;

        Deactivate();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag("TransportModule"))
        {
            if (other.gameObject.GetComponent<TransportModule>())
            {
                other.gameObject.GetComponent<TransportModule>().DeliverCargo(dropPoint == null ? c_Transform: dropPoint);
            }
        }
    }
}
