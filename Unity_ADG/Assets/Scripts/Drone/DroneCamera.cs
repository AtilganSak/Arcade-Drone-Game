using UnityEngine;
using UnityEngine.UI;

public class DroneCamera : MonoBehaviour
{
    public bool mobile;

    public float smoothing;
    public float damping = 4;

    public float xSpeed = 10;
    public float ySpeed = 10;

    public bool invertX;
    public bool invertY = true;

    [Header("Screen")]
    public bool showScreenLimit;
    public float touchableYLimitOffset = 300;

    [Header("Reset Camera")]
    public float returnBackSpeedTreshold = 0.1f;
    public float returnBackTime = 6;
    public float returnBackSpeed = 5;

    [Header("Rotate Limits")]
    public float yMinLimit = -90;
    public float yMaxLimit = 90;

    [Header("Zoom")]
    public float zoomRate = 40;
    public float maxZoomDistance = 20;
    public float minZoomDistance = .6f;
    public float zoomDampening = 10;

    public InputField zoomRateField;
    public InputField zoomDampingField;
    public InputField minDisField;
    public InputField maxDisField;

    float xDeg;
    float yDeg;
    float counter;
    float currentDistance;
    float desiredDistance;

    bool returnBack;
    bool firstTrueTouched;
    bool isTouching;

    DroneController droneController;

    Transform cameraTransform;
    Transform c_Transform;

    Quaternion resetRotation;
    Quaternion desiredRotation;

    Vector3 position;
    Vector2 touchZeroPrevPos;
    Vector2 touchOnePrevPos;

    Touch touchZero;
    Touch touchOne;

    private void OnEnable()
    {
        c_Transform = transform;
        resetRotation = c_Transform.localRotation;

        cameraTransform = GetComponentInChildren<Camera>().transform;
        position = cameraTransform.position;

        droneController = GetComponentInParent<DroneController>();
    }
    private void Start()
    {
        desiredRotation = c_Transform.localRotation;

        currentDistance = Vector3.Distance(cameraTransform.position, c_Transform.position);
        desiredDistance = Vector3.Distance(cameraTransform.position, c_Transform.position);

        xDeg = Vector3.Angle(Vector3.right, c_Transform.right);
        yDeg = Vector3.Angle(Vector3.up, c_Transform.up);

        zoomRateField.text = zoomRate.ToString();
        zoomDampingField.text = zoomDampening.ToString();
        minDisField.text = minZoomDistance.ToString();
        maxDisField.text = maxZoomDistance.ToString();
    }
    public void ChangedZoomReate(string s)
    {
        zoomRate = float.Parse(s);
    }
    public void ChangedZoomDamp(string s)
    {
        zoomDampening = float.Parse(s);
    }
    public void ChangedMinDis(string s)
    {
        minZoomDistance = float.Parse(s);
    }
    public void ChangedMaxDis(string s)
    {
        maxZoomDistance = float.Parse(s);
    }
    private void Update()
    {
        if (!isTouching)
        {
            if (droneController.Speed > returnBackSpeedTreshold && !returnBack)
            {
                counter += Time.deltaTime;
                if (counter > returnBackTime)
                {
                    returnBack = true;
                    counter = 0;
                }
            }

            if (returnBack)
            {
                desiredRotation = Quaternion.Slerp(desiredRotation, resetRotation, Time.deltaTime * returnBackSpeed);
                if (desiredRotation == resetRotation)
                    returnBack = false;
            }
        }

#if  UNITY_EDITOR || UNITY_STANDALONE
        if (!mobile)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Input.mousePosition.y > Screen.height / 2 - touchableYLimitOffset)
                {
                    firstTrueTouched = true;
                    isTouching = true;
                    counter = 0;
                    returnBack = false;

                    desiredRotation = c_Transform.localRotation;
                }
                else
                {
                    firstTrueTouched = false;
                }
            }
            if (Input.GetMouseButton(0))
            {
                if (firstTrueTouched)
                {
                    if (!invertX)
                        xDeg += Input.GetAxis("Mouse X");
                    else
                        xDeg -= Input.GetAxis("Mouse X");
                    if (!invertY)
                        yDeg += Input.GetAxis("Mouse Y");
                    else
                        yDeg -= Input.GetAxis("Mouse Y");

                    ClampAndLimit();

                    desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);

                    c_Transform.localRotation = Quaternion.Slerp(c_Transform.localRotation, desiredRotation, Time.deltaTime * smoothing);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                firstTrueTouched = false;
                isTouching = false;
            }
            if (c_Transform.localRotation != desiredRotation)
            {
                c_Transform.localRotation = Quaternion.Slerp(c_Transform.localRotation, desiredRotation, Time.deltaTime * damping);
            }
        }
#endif
#if UNITY_EDITOR || UNITY_ANDROID
        if (mobile)
        {
            #region Rotate
            if (Input.touchCount == 1)
            {
                touchZero = Input.GetTouch(0);

                if (touchZero.phase == TouchPhase.Began)
                {
                    if (touchZero.position.y > Screen.height / 2 - touchableYLimitOffset)
                    {
                        firstTrueTouched = true;
                        isTouching = true;
                        counter = 0;
                        returnBack = false;

                        desiredRotation = c_Transform.localRotation;
                    }
                    else
                    {
                        firstTrueTouched = false;
                    }
                }
                if (touchZero.phase == TouchPhase.Moved)
                {
                    if (firstTrueTouched)
                    {
                        if (!invertX)
                            xDeg += Input.touches[0].deltaPosition.x * Time.deltaTime * xSpeed;
                        else
                            xDeg -= Input.touches[0].deltaPosition.x * Time.deltaTime * xSpeed;
                        if (!invertY)
                            yDeg += Input.touches[0].deltaPosition.y * Time.deltaTime * ySpeed;
                        else
                            yDeg -= Input.touches[0].deltaPosition.y * Time.deltaTime * ySpeed;

                        ClampAndLimit();

                        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);

                        c_Transform.localRotation = Quaternion.Slerp(c_Transform.localRotation, desiredRotation, Time.deltaTime * smoothing);
                    }
                }
                if (touchZero.phase == TouchPhase.Ended)
                {
                    firstTrueTouched = false;
                    isTouching = false;
                }
            }
            if (c_Transform.localRotation != desiredRotation)
            {
                c_Transform.localRotation = Quaternion.Slerp(c_Transform.localRotation, desiredRotation, Time.deltaTime * damping);
            }
            #endregion

            #region Zoom
            if (Input.touchCount == 2)
            {
                if (firstTrueTouched)
                {
                    // Store both touches.
                    touchZero = Input.GetTouch(0);
                    touchOne = Input.GetTouch(1);

                    // Find the position in the previous frame of each touch.
                    touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).sqrMagnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).sqrMagnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    // affect the desired Zoom distance if we pinch
                    desiredDistance += deltaMagnitudeDiff * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance) * 0.001f;
                    //clamp the zoom min/max
                    desiredDistance = Mathf.Clamp(desiredDistance, minZoomDistance, maxZoomDistance);
                    // For smoothing of the zoom, lerp distance
                    currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

                    position = c_Transform.position - (cameraTransform.rotation * Vector3.forward * currentDistance);
                    cameraTransform.position = Vector3.Lerp(cameraTransform.position, position, Time.deltaTime * zoomDampening);
                }
            }
            #endregion
        }
#endif
    }
    void ClampAndLimit()
    {
        if (yDeg < -360)
            yDeg += 360;
        if (yDeg > 360)
            yDeg -= 360;

        if (yDeg < yMinLimit)
            yDeg = yMinLimit;
        else if (yDeg > yMaxLimit)
            yDeg = yMaxLimit;
    }
#if UNITY_EDITOR
    private void OnGUI()
    {
        if(showScreenLimit)
            GUI.Box(new Rect(0, Screen.height / 2 + touchableYLimitOffset, Screen.width, 10), "Box");
    }
#endif
}
