using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] internal bool connectPath = true;

    public float movingSpeed = 10;

    public float rotationSpeed = 100;

    public float reachDistance = 0.5F;

    public Vector3 offset;

    public bool reverseFollow;

    public FinishingMove finishingMove;

    public RoadPath roadPath { get; private set; }

    int currentDirection;
    int currentIndex;

    internal float c_MoveSpeed;

    internal Transform c_Transform;

    Vector3 relDirection;

    public enum FinishingMove
    {
        Loop,
        Inverse,
        Destroy
    }

    private void OnEnable()
    {
        c_MoveSpeed = movingSpeed;

        c_Transform = transform;

        VirtualOnEnable();
    }
    private void OnDisable()
    {
        VirtualOnDisable();
    }
    private void Start()
    {
        if (reverseFollow)
        {
            currentDirection = -1;
        }
        else
        {
            currentDirection = 1;
        }

        FindPath();

        VirtualStart();
    }
    private void OnDestroy()
    {
        VirtualOnDestroy();
    }

    public virtual void OnUpdate() 
    {
        RotateTo();
        MoveTo();
    }
    protected virtual void VirtualStart() { }
    protected virtual void VirtualOnEnable() { }
    protected virtual void VirtualOnDisable() { }
    protected virtual void VirtualOnDestroy() { }

    public void SetPath(RoadPath path, int pathIndex)
    {
        roadPath = path;
        currentIndex = pathIndex;
        connectPath = true;
    }
    void FindPath()
    {
        RoadPath[] paths = FindObjectsOfType<RoadPath>();
        if (paths != null && paths.Length > 0)
        {
            RoadPath path = null;
            Vector3 nearPoint = paths[0].GetPoint(0);
            float nearDis = (c_Transform.position - nearPoint).sqrMagnitude;
            int index = 0;
            for (int k = 0; k < paths.Length; k++)
            {
                if (paths[k].points.Length > 0)
                {
                    for (int i = 0; i < paths[k].points.Length; i++)
                    {
                        if ((c_Transform.position - paths[k].GetPoint(i)).sqrMagnitude < nearDis)
                        {
                            nearPoint = paths[k].GetPoint(i);
                            nearDis = (c_Transform.position - paths[k].GetPoint(i)).sqrMagnitude;
                            index = i;
                            path = paths[k];
                        }
                    }
                }
            }
            c_Transform.position = nearPoint;
            currentIndex = index;
            roadPath = path;
        }
    }
    void MoveTo()
    {
        c_Transform.position = Vector3.MoveTowards(c_Transform.position, roadPath.GetPoint(currentIndex), Time.smoothDeltaTime * movingSpeed);

        if ((c_Transform.position - roadPath.GetPoint(currentIndex)).sqrMagnitude < reachDistance)
        {
            if (currentDirection == 1)
            {
                if (currentIndex + 1 < roadPath.points.Length)
                {
                    currentIndex++;
                }
                else
                {
                    if (finishingMove == FinishingMove.Loop)
                    {
                        currentIndex = 0;

                        if(!roadPath.lockPath)
                            c_Transform.position = roadPath.GetPoint(currentIndex);
                    }
                    else if (finishingMove == FinishingMove.Inverse)
                    {
                        currentDirection = -1;
                    }
                    else if (finishingMove == FinishingMove.Destroy)
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else if (currentDirection == -1)
            {
                if (currentIndex - 1 >= 0)
                {
                    currentIndex--;
                }
                else
                {
                    if (finishingMove == FinishingMove.Loop)
                    {
                        currentIndex = roadPath.points.Length - 1;

                        if (!roadPath.lockPath)
                            c_Transform.position = roadPath.GetPoint(currentIndex);
                    }
                    else if (finishingMove == FinishingMove.Inverse)
                    {
                        currentDirection = 1;
                    }
                    else if (finishingMove == FinishingMove.Destroy)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
    void RotateTo()
    {
        relDirection = (c_Transform.position - roadPath.GetPoint(currentIndex));
        c_Transform.rotation = Quaternion.RotateTowards(c_Transform.rotation, Quaternion.LookRotation(-relDirection), Time.smoothDeltaTime * rotationSpeed);
    }
}
