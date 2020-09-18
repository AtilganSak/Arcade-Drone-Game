using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    public bool receivable = true;

    public DeliveryPlace deliveryPlace;

    public GameObject indicators;

    public TMP_Text distanceText;

    public bool isCarrying { get; private set; }

    public CargoPoint startPoint;
    public Transform Transform { get; private set; }
    public Collider Collider { get; private set; }
    public GameObject GameObject { get; private set; }
    public Transform downPivot { get; private set; }
    public Transform upPivot { get; private set; }

    public int listIndex { get; set; }
    public bool spawning { get; private set; }

    public bool visibleByCamera;
    public bool startTimer;
    public bool waitedForTime;

    public float timer;
    float waitTime;

    MeshRenderer meshRenderer;
    RaycastHit rayCastHit;
    WaitForSeconds forSeconds;

    Camera camera;

    private void OnEnable()
    {
        camera = Camera.main;

        Transform = transform;
        GameObject = gameObject;
        Collider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();

        forSeconds = new WaitForSeconds(0.5F);

        upPivot = Transform.root;
        downPivot = upPivot.GetChild(0);

        if (deliveryPlace)
            deliveryPlace.connectedCargo = true;
    }
    private void Start()
    {
        CalculateDistance();
    }
    private void OnBecameVisible()
    {
        visibleByCamera = true;
    }
    private void OnBecameInvisible()
    {
        visibleByCamera = false;
    }
    private void Update()
    {
        if (startTimer)
        {
            if (!waitedForTime)
            {
                timer += Time.deltaTime;
                if (timer > waitTime)
                {
                    timer = 0;
                    waitedForTime = true;
                    meshRenderer.enabled = true;
                }
            }
            if(waitedForTime)
            {
                startTimer = false;

                waitedForTime = false;

                Collider.enabled = true;
                meshRenderer.enabled = true;
                spawning = false;

                DoReceivable();
            }
        }
    }
    public void DoReceivable()
    {
        receivable = true;

        indicators.SetActive(true);
    }
    public void DoUnreachable()
    {
        receivable = false;

        indicators.SetActive(false);
    }
    public void DoReceivableWithTime(float time)
    {
        DoUnreachable();

        Collider.enabled = false;
        meshRenderer.enabled = false;
        spawning = true;

        waitTime = time;

        startTimer = true;
    }
    public void Received()
    {
        isCarrying = true;

        deliveryPlace.Activate();

        indicators.SetActive(false);
    }
    public void Delivered()
    {
        isCarrying = false;

        deliveryPlace.DeliveredCargo(this);
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
    public void CalculateDistance()
    {
        if (deliveryPlace != null)
            distanceText.text = Mathf.RoundToInt(Vector3.Distance(Transform.position, deliveryPlace.transform.position)).ToString();
        else
            distanceText.gameObject.SetActive(false);
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
