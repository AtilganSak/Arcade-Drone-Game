using UnityEditor;
using UnityEngine;

public class Police : MonoBehaviour
{
    public string targetTag = "Player";

    public float rotateSpeed = 10;
    public float moveSpeed = 100;
    public float maxFollowDistance = 30;
    public float arrestDistance = 3;

    [Header("Collision Detection")]
    public float rayLength = 5;
    public float rayPosOffset = 10;
    public LayerMask detectionLayer;

    Transform targetTR;
    Transform c_Transform;

    SphereCollider sphereCollider;
    MovingPolice movingComponent;

    bool stop;
    bool detectedTarget;
    bool returnBack;

    Vector3 startPosition;
    Vector3 destination;

    RaycastHit hit;
    RaycastHit hit2;

    private void OnEnable()
    {
        c_Transform = transform;
        movingComponent = GetComponent<MovingPolice>();
    }
    private void Start()
    {
        startPosition = c_Transform.position;
    }
    private void FixedUpdate()
    {
        if (stop) return;

        if (detectedTarget)
        {
            destination = targetTR.position;

            CheckMaxDistance();
            CheckLookTarget();
            CollisionDetection();
            RotateTo(targetTR.position);
            MoveTo(destination);
        }
        if(returnBack)
        {
            destination = startPosition;

            CollisionDetection();
            RotateTo(destination);
            MoveTo(destination, true);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (detectedTarget) return;
        if (other.gameObject.tag == targetTag)
        {
            targetTR = other.transform;

            movingComponent.connectPath = false;
            detectedTarget = true;
            returnBack = false;
        }
    }
    private void OnDrawGizmos()
    {
        if (detectedTarget)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(c_Transform.position, targetTR.position);
        }
        if (sphereCollider == null)
            sphereCollider = GetComponent<SphereCollider>();
        Gizmos.DrawWireSphere(transform.position, sphereCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z));
    }
    void CheckMaxDistance()
    {
        if ((targetTR.position - c_Transform.position).sqrMagnitude > maxFollowDistance)
        {
            detectedTarget = false;
            returnBack = true;
        }
    }
    void CheckLookTarget()
    {
        if (Physics.Linecast(c_Transform.position, targetTR.position, LayerMask.NameToLayer("Player"), QueryTriggerInteraction.Ignore))
        {
            detectedTarget = false;
            returnBack = true;
        }
    }
    void CollisionDetection()
    {
        if (Physics.Raycast(c_Transform.position + c_Transform.forward * rayPosOffset, -c_Transform.up, out hit2, rayLength, detectionLayer))
        {
            if (hit2.distance <= rayLength)
                destination.y += 100;
            //destination += (c_Transform.position - hit2.point) * rayLength * 500;
        }
        if (Physics.Raycast(c_Transform.position + c_Transform.forward * rayPosOffset, c_Transform.up, out hit2, rayLength, detectionLayer))
        {
            if (hit2.distance <= rayLength)
                destination.y -= 100;
            //destination -= (hit2.point - c_Transform.position) * rayLength * 500;
        }
        if (Physics.Raycast(c_Transform.position + c_Transform.forward * rayPosOffset, c_Transform.right, out hit2, rayLength, detectionLayer))
        {
            if (hit2.distance <= rayLength)
                destination.x += 100;
            //destination += (c_Transform.position - hit2.point) * rayLength * 500;
        }
        if (Physics.Raycast(c_Transform.position + c_Transform.forward * rayPosOffset, -c_Transform.right, out hit2, rayLength, detectionLayer))
        {
            if (hit2.distance <= rayLength)
                destination.x -= 100;
                //destination -= (hit2.point - c_Transform.position) * rayLength * 50;
        }
#if UNITY_EDITOR
        Debug.DrawRay(c_Transform.position + c_Transform.forward * rayPosOffset, -c_Transform.up * rayLength, Color.red);
        Debug.DrawRay(c_Transform.position + c_Transform.forward * rayPosOffset, c_Transform.up * rayLength, Color.red);
        Debug.DrawRay(c_Transform.position + c_Transform.forward * rayPosOffset, c_Transform.right * rayLength, Color.red);
        Debug.DrawRay(c_Transform.position + c_Transform.forward * rayPosOffset, -c_Transform.right * rayLength, Color.red);
#endif
    }
    void RotateTo(Vector3 targetPosition)
    {
        if (c_Transform.localRotation != Quaternion.LookRotation(targetPosition - c_Transform.position))
        {
            c_Transform.localRotation = Quaternion.RotateTowards(c_Transform.localRotation, Quaternion.LookRotation(targetPosition - c_Transform.position), Time.smoothDeltaTime * rotateSpeed);
        }
    }
    void MoveTo(Vector3 targetPosition, bool back = false)
    {
        if ((targetPosition - c_Transform.position).sqrMagnitude > arrestDistance)
        {
            c_Transform.localPosition = Vector3.MoveTowards(c_Transform.localPosition, targetPosition, Time.smoothDeltaTime * moveSpeed);
        }
        else
        {
            returnBack = false;
            if (!back)
            {
                //stop = true;
                Debug.LogError("You busted!");
            }
            else
            {
                movingComponent.connectPath = true;
            }
        }
    }
}
