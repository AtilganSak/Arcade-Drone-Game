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

    public LayerMask ground;

    //True drone speed.
    public float Speed { get; private set; }
    //The drone's height from the ground.
    public float Altitude { get; private set; }

    float angle;
    float ThrustInput, TiltInput, LiftInput, RotateInput;
    float verticalAnim;
    float horizontalAnim;
    float rotateAnim;

    Quaternion rotation;
    Vector3 direction;
    Vector3 currentVelocity;
    Rigidbody rigidbody;
    Animator anim;
    Transform myTransform;
    RaycastHit raycastHit;

    private void Start()
    {
        myTransform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        GetInputs();

        SetAnimation();

        Move();

        Rotation();

        CalculateAltitude();

        CalculateSpeed();
    }

    void GetInputs()
    {
        LiftInput = Input.GetAxis("Lift");
        RotateInput = Input.GetAxis("Rotate");
        ThrustInput = Input.GetAxis("Vertical");
        TiltInput = Input.GetAxis("Horizontal");

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
        direction = new Vector3(TiltInput * movement, LiftInput * lift, ThrustInput * movement);

        rigidbody.AddRelativeForce(direction, ForceMode.Impulse);

        currentVelocity = rigidbody.velocity;

        currentVelocity.x = Mathf.Clamp(currentVelocity.x, -movement, movement);
        currentVelocity.y = Mathf.Clamp(currentVelocity.y, -lift, lift);
        currentVelocity.z = Mathf.Clamp(currentVelocity.z, -movement, movement);

        rigidbody.velocity = currentVelocity;
    }
    void Rotation()
    {
        angle += Input.GetAxis("Rotate");

        rotation = Quaternion.AngleAxis(angle, Vector3.up);
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, rotation, rotationSpeed * Time.deltaTime);
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
}
