using System.ComponentModel;
using UnityEditor;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] internal bool connectPath = true;

    public Transform visualChild;

    public float movingSpeed = 10;
    public float rotationSpeed = 100;
    public float reachDistance = 0.5F;

    public bool reverseFollow;

    public FinishingMove finishingMove;
    
    [SerializeField] bool useOffset;
    [SerializeField] float offsetValue;
    [SerializeField] bool randomize;
    [SerializeField] bool circle;
    [SerializeField] bool clampOffset;
    [SerializeField] float circleAngle;
    [SerializeField] float lineerOffsetValue;

    public RoadPath roadPath { get; private set; }

    float currentOfffset;
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
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (useOffset)
        {
            if (circle)
            {
                Handles.CircleHandleCap(-1, transform.position, Quaternion.LookRotation(transform.forward), offsetValue, EventType.Repaint);
            }
            else
            {
                Vector3 rightPos = transform.position + transform.right * offsetValue;
                Vector3 leftPos = transform.position - transform.right * offsetValue;

                Gizmos.DrawSphere(rightPos, 0.3F);
                Gizmos.DrawSphere(leftPos, 0.3F);

                if (!clampOffset)
                    Gizmos.DrawLine(rightPos, leftPos);
            }

        }
    }
#endif

    public virtual void OnUpdate()
    {
        if (connectPath)
        {
            RotateTo();
            MoveTo();
        }
    }
    protected virtual void VirtualStart() { }
    protected virtual void VirtualOnEnable() { }
    protected virtual void VirtualOnDisable() { }
    protected virtual void VirtualOnDestroy() { }

    void FindPath()
    {
        RoadPath[] paths = FindObjectsOfType<RoadPath>();
        if (paths != null && paths.Length > 0)
        {
            RoadPath path = null;
            if (paths[0].points.Length > 1)
                path = paths[0];
            Vector3 nearPoint = paths[0].GetPoint(0);
            float nearDis = (c_Transform.position - nearPoint).sqrMagnitude;
            int index = 0;
            for (int k = 0; k < paths.Length; k++)
            {
                if (paths[k].points.Length < 2)
                    continue;
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
            if (roadPath == null)
                connectPath = false;
            else
                connectPath = true;
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

                        if (!roadPath.lockPath)
                        {
                            c_Transform.position = roadPath.GetPoint(currentIndex);
                            c_Transform.rotation = Quaternion.LookRotation((roadPath.GetPoint(1) - roadPath.GetPoint(0)));

                            if(useOffset)
                                ChangeOffsetPosition();
                        }
                    }
                    else if (finishingMove == FinishingMove.Inverse)
                    {
                        currentDirection = -1;
                        c_Transform.rotation = Quaternion.LookRotation((roadPath.GetPoint(currentIndex - 1) - (roadPath.GetPoint(currentIndex))));
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
                        {
                            c_Transform.position = roadPath.GetPoint(currentIndex);
                            c_Transform.rotation = Quaternion.LookRotation((roadPath.GetPoint(roadPath.points.Length - 2) - (roadPath.GetPoint(roadPath.points.Length - 1))));

                            if (useOffset)
                                ChangeOffsetPosition();
                        }
                    }
                    else if (finishingMove == FinishingMove.Inverse)
                    {
                        currentDirection = 1;
                        c_Transform.rotation = Quaternion.LookRotation((roadPath.GetPoint(currentIndex + 1) - (roadPath.GetPoint(currentIndex))));
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
        relDirection = (c_Transform.localPosition - roadPath.GetPoint(currentIndex));
        if (relDirection != Vector3.zero)
            c_Transform.localRotation = Quaternion.RotateTowards(c_Transform.localRotation, Quaternion.LookRotation(-relDirection), Time.smoothDeltaTime * rotationSpeed);
    }
    void ChangeOffsetPosition()
    {
        if (visualChild)
        {
            if (randomize)
            {
                if (circle)
                {
                    visualChild.localPosition = new Vector3(Mathf.Sin((Mathf.PI / 180) * Random.Range(0, 360)) * offsetValue, Mathf.Cos((Mathf.PI / 180) * Random.Range(0, 360)) * offsetValue, 0);
                }
                else
                {
                    if (!clampOffset)
                        visualChild.position = visualChild.position + visualChild.right * Random.Range(-offsetValue, offsetValue);
                    else
                    {
                        int rnd = Random.Range(0, 2);
                        if (rnd == 0)
                        {
                            if (currentOfffset != -offsetValue)
                            {
                                visualChild.position = visualChild.position + visualChild.right * -offsetValue;
                                currentOfffset = -offsetValue;
                            }
                        }
                        else if (rnd == 1)
                        {
                            if (currentOfffset != offsetValue)
                            {
                                visualChild.position = visualChild.position + visualChild.right * offsetValue;
                                currentOfffset = offsetValue;
                            }
                        }
                    }
                }
            }
        }
    }
}
[CanEditMultipleObjects, CustomEditor(typeof(MovingObject), true)]
public class MovingObjectEditor : Editor
{
    MovingObject script { get => target as MovingObject; }

    SerializedObject s_script;
    SerializedProperty s_connectPath;
    SerializedProperty s_useOffset;
    SerializedProperty s_randomize;
    SerializedProperty s_circle;
    SerializedProperty s_offsetValue;
    SerializedProperty s_clamp;
    SerializedProperty s_circleAngle;
    SerializedProperty s_lineerOffsetValue;

    bool appleOffsetPosition;

    private void OnEnable()
    {
        s_script = new SerializedObject(script);
        s_connectPath = s_script.FindProperty("connectPath");
        s_useOffset = s_script.FindProperty("useOffset");
        s_randomize = s_script.FindProperty("randomize");
        s_circle = s_script.FindProperty("circle");
        s_offsetValue = s_script.FindProperty("offsetValue");
        s_clamp = s_script.FindProperty("clampOffset");
        s_circleAngle = s_script.FindProperty("circleAngle");
        s_lineerOffsetValue = s_script.FindProperty("lineerOffsetValue");

        if (PlayerPrefs.HasKey("applyoffset"))
        {
            appleOffsetPosition = PlayerPrefs.GetInt("applyoffset") == 1 ? true : false;
        }
    }
    public override void OnInspectorGUI()
    {
        s_script.Update();
        GUI.backgroundColor = Color.cyan;
        using (new EditorGUILayout.VerticalScope("Box"))
        {
            EditorGUILayout.PropertyField(s_connectPath);
        }

        EditorGUI.BeginChangeCheck();
        using (new EditorGUILayout.VerticalScope("Box"))
        {
            script.visualChild = (Transform)EditorGUILayout.ObjectField("Visual Child", script.visualChild, typeof(Transform), true);
            script.movingSpeed = EditorGUILayout.FloatField("Moving Speed", script.movingSpeed);
            script.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", script.rotationSpeed);
            script.reachDistance = EditorGUILayout.FloatField("Reac Distance", script.reachDistance);
            script.finishingMove = (MovingObject.FinishingMove)EditorGUILayout.EnumPopup("Finishing Move", script.finishingMove);
        }
        using (new EditorGUILayout.VerticalScope("Box"))
        {
            EditorGUILayout.PropertyField(s_useOffset);
            if (s_useOffset.boolValue)
            {
                EditorGUILayout.PropertyField(s_randomize);
                appleOffsetPosition = EditorGUILayout.Toggle("Apply Offset", appleOffsetPosition);
                EditorGUILayout.PropertyField(s_circle);
                EditorGUILayout.PropertyField(s_offsetValue);
                if (!s_circle.boolValue)
                {
                    if (s_randomize.boolValue)
                    {
                        EditorGUILayout.PropertyField(s_clamp);
                    }
                    else
                    {
                        s_lineerOffsetValue.floatValue = EditorGUILayout.Slider("Lineer Offset Value", s_lineerOffsetValue.floatValue, 0, 1);
                    }
                }
                else
                {
                    if (!s_randomize.boolValue)
                    {
                        s_circleAngle.floatValue = EditorGUILayout.Slider("Circle Angle", s_circleAngle.floatValue, 0, 360);
                    }
                }
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            PlayerPrefs.SetInt("applyoffset", appleOffsetPosition ? 1 : 0);

            Undo.RecordObject(script.visualChild.transform, "changedOffsetpos");
            if (appleOffsetPosition)
            {
                if (script.visualChild)
                {
                    if (s_circle.boolValue)
                    {
                        script.visualChild.localPosition = new Vector3(Mathf.Sin((Mathf.PI / 180) * s_circleAngle.floatValue) * s_offsetValue.floatValue, Mathf.Cos((Mathf.PI / 180) * s_circleAngle.floatValue) * s_offsetValue.floatValue, 0);
                    }
                    else
                    {
                        Vector3 rightPos = script.transform.position + script.transform.right * s_offsetValue.floatValue;
                        Vector3 leftPos = script.transform.position - script.transform.right * s_offsetValue.floatValue;
                        script.visualChild.position = Vector3.Lerp(leftPos , rightPos, s_lineerOffsetValue.floatValue);
                    }
                }
            }
            else
            {
                script.visualChild.transform.localPosition = Vector3.zero;
            }
            EditorUtility.SetDirty(script);
        }
        s_script.ApplyModifiedProperties();
    }
}