using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneController : MonoBehaviour
{
    [Tooltip("The speed of movement up or down.")]
    public float lift = 5;
    [Tooltip("Velocity movement speed.")]
    public float movement = 5;

    [Tooltip("Speed of rotation itself.")]
    public float rotationSpeed = 5;
    [Tooltip("Speed ​​of transition between two animations.")]
    public float blendSpeed = 2;

    public bool rawInput;

    public LayerMask ground;

    //True drone speed.
    public float Speed { get; private set; }
    //The drone's height from the ground.
    public float Altitude { get; private set; }

    float ThrustInput;
    float TiltInput;
    float LiftInput; 
    float RotateInput;
    float verticalAnim;
    float horizontalAnim;
    float rotateAnim;

    Quaternion rotation;
    Rigidbody rigidbody;
    Animator anim;
    Transform myTransform;
    RaycastHit raycastHit;
    Vector3 upAngle;

    private void Start()
    {
        rotation = Quaternion.identity;
        myTransform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Debug();

        GetInputs();

        SetAnimation();

        Move();

        Rotation();

        CalculateAltitude();

        CalculateSpeed();
    }

    void GetInputs()
    {
        if (!rawInput)
        {
            LiftInput = Input.GetAxis("Lift");
            RotateInput = Input.GetAxis("Rotate");
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
        verticalAnim = anim.GetFloat("Vertical");
        horizontalAnim = anim.GetFloat("Horizontal");
        rotateAnim = anim.GetFloat("Rotate");
    }
    void SetAnimation()
    {
        anim.SetFloat("Vertical", Mathf.Clamp(Mathf.Lerp(verticalAnim, ThrustInput, blendSpeed * Time.deltaTime), -1, 1));
        anim.SetFloat("Horizontal", Mathf.Clamp(Mathf.Lerp(horizontalAnim, TiltInput, blendSpeed * Time.deltaTime), -1, 1));
        anim.SetFloat("Rotate", Mathf.Clamp(Mathf.Lerp(rotateAnim, RotateInput, blendSpeed * Time.deltaTime), -1, 1));
    }
    void Move()
    {
        rigidbody.AddRelativeForce(TiltInput * movement * Time.deltaTime, LiftInput * lift * Time.deltaTime, ThrustInput * movement * Time.deltaTime);
    }
    void Rotation()
    {
        upAngle.y = RotateInput;

        rotation = myTransform.localRotation * Quaternion.Euler(upAngle);

        myTransform.localRotation = Quaternion.Slerp(myTransform.localRotation, rotation, Time.deltaTime * rotationSpeed);
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
        Lebug.Log("Lift", LiftInput, "Drone");
        Lebug.Log("Rotate", RotateInput, "Drone");
        Lebug.Log("Vertical", ThrustInput, "Drone");
        Lebug.Log("Horizontal", TiltInput, "Drone");
    }
}
