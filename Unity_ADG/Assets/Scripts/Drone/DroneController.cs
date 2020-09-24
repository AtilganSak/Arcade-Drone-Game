using UnityEngine;

public class DroneController : MonoBehaviour
{
    public bool isActive = true;

    [Tooltip("The speed of movement up or down.")]
    public float lift = 5;
    [Tooltip("Velocity movement speed.")]
    public float movement = 5;

    [Tooltip("Speed of rotation itself.")]
    public float rotationSpeed = 5;
    [Tooltip("Speed ​​of transition between two animations.")]
    public float blendSpeed = 2;
    [Tooltip("Joysticks dead zone.")]
    public float kEpsilon = 0.1F;

    public bool FOVEffect = true;
    [ConditionalField("FOVEffect",true)]
    public float desireFOVAmount = 70;
    [ConditionalField("FOVEffect", true)]
    public float transitionFOVSmooth = 3;

    public bool rawInput;
    public ForceMode forceMode;
    public LayerMask ground;

    public bool AnyInput { get => ThrustInput != 0 || TiltInput != 0 || LiftInput != 0; }
    public bool IsMoving { get => Speed > 0; }
    //True drone speed.
    public float Speed { get; private set; }
    //The drone's height from the ground.
    public float Altitude { get; private set; }

    public Transform myTransform { get; private set; }

    float ThrustInput;
    float TiltInput;
    float LiftInput; 
    float RotateInput;
    float verticalAnim;
    float horizontalAnim;
    float rotateAnim;
    float baseCameraFOV;

    Camera camera;
    Rigidbody rigidbody;
    Animator anim;
    RaycastHit raycastHit;
    Quaternion deltaRotation;

    private void Start()
    {
        camera = Camera.main;
        baseCameraFOV = camera.fieldOfView;
        myTransform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isActive) return;

        //Debug();

        

        //SetAnimation();

        CameraFOVEffect();

        CalculateAltitude();

        CalculateSpeed();
    }
    private void FixedUpdate()
    {
        GetInputs();

        Move();

        Rotation();
    }

    void GetInputs()
    {
        if (!rawInput)
        {
            LiftInput = Input.GetAxis("Lift");
            if(LiftInput < 0)
            {
                if (LiftInput > -kEpsilon)
                    LiftInput = 0;
            }
            else
            {
                if (LiftInput < kEpsilon)
                    LiftInput = 0;
            }
            RotateInput = Input.GetAxis("Rotate");
            if (RotateInput < 0)
            {
                if (RotateInput > -kEpsilon)
                    RotateInput = 0;
            }
            else
            {
                if (RotateInput < kEpsilon)
                    RotateInput = 0;
            }
            ThrustInput = Input.GetAxis("Vertical");
            TiltInput = Input.GetAxis("Horizontal");
        }
        else
        {
            LiftInput = Input.GetAxisRaw("Lift");
            RotateInput = Input.GetAxisRaw("Rotate");
            ThrustInput = Input.GetAxisRaw("Vertical");
            TiltInput = Input.GetAxisRaw("Horizontal");
        }
        //verticalAnim = anim.GetFloat("Vertical");
        //horizontalAnim = anim.GetFloat("Horizontal");
        //rotateAnim = anim.GetFloat("Rotate");
    }
    void CameraFOVEffect()
    {
        if (!FOVEffect) return;

        if (ThrustInput > 0)
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, desireFOVAmount, Time.smoothDeltaTime * transitionFOVSmooth);
        }
        else
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, baseCameraFOV, Time.smoothDeltaTime * transitionFOVSmooth);
        }
    }
    void SetAnimation()
    {
        anim.SetFloat("Vertical", Mathf.Clamp(Mathf.Lerp(verticalAnim, ThrustInput, blendSpeed * Time.smoothDeltaTime), -1, 1));
        anim.SetFloat("Horizontal", Mathf.Clamp(Mathf.Lerp(horizontalAnim, TiltInput, blendSpeed * Time.smoothDeltaTime), -1, 1));
        anim.SetFloat("Rotate", Mathf.Clamp(Mathf.Lerp(rotateAnim, RotateInput, blendSpeed * Time.smoothDeltaTime), -1, 1));
    }
    void Move()
    {
        rigidbody.AddRelativeForce(TiltInput * movement * Time.smoothDeltaTime, LiftInput * lift * Time.smoothDeltaTime, ThrustInput * movement * Time.smoothDeltaTime, forceMode);
    }
    void Rotation()
    {
        deltaRotation = Quaternion.Euler(0, RotateInput * Time.smoothDeltaTime * rotationSpeed, 0);
        rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
    }
    void CalculateAltitude()
    {
        if (Physics.Raycast(myTransform.position, Vector3.down, out raycastHit, 9000, ground, QueryTriggerInteraction.Ignore))
        {
            Altitude = raycastHit.distance;
        }
    }
    void CalculateSpeed()
    {
        Speed = rigidbody.velocity.sqrMagnitude;
    }
    void Debug()
    {
        Lebug.Log("Speed", Speed, "Drone");
        Lebug.Log("Altitude", Altitude, "Drone");

        Lebug.Log("Lift", LiftInput, "Drone");
        Lebug.Log("Roll", RotateInput, "Drone");
        Lebug.Log("Thrust", ThrustInput, "Drone");
        Lebug.Log("Tilt", TiltInput, "Drone");
    }
}
