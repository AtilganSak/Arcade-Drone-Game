using UnityEngine;

public class DeliveryPlace : MonoBehaviour
{
    public Transform dropPoint;

    public GameObject visual;

    public bool connectedCargo { get; set; }

    public int listIndex { get; set; }

    bool isActive;

    //Target indicator;
    CargoSystem cargoSystem;
    Transform c_Transform;
    Collider collider;

    private void OnEnable()
    {
        //indicator = GetComponent<Target>();
        collider = GetComponent<Collider>();
        cargoSystem = FindObjectOfType<CargoSystem>();

        c_Transform = transform;
    }
    private void Start()
    {
        if (!connectedCargo)
            cargoSystem.AddEmptyDeliveryPlace(this);

        Deactivate();
    }
    public void Activate()
    {
        collider.enabled = true;

        visual.SetActive(true);

        //indicator.enabled = true;

        isActive = true;
    }
    public void Deactivate()
    {
        collider.enabled = false;

        visual.SetActive(false);

        //indicator.enabled = false;

        isActive = false;
    }
    public void DeliveredCargo(Cargo cargo)
    {
        connectedCargo = false;

        Invoke("Spawned", 5);

        Deactivate();
    }
    void Spawned()
    {
        cargoSystem.AddEmptyDeliveryPlace(this);
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
