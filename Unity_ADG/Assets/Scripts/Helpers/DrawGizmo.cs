using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum SyncType{ ScynToObjectSize = 0, ScynToColliderSize = 1, Free = 2 }
public enum ColliderType{ Box = 0, Sphere = 1, Capsule = 2 }

public class DrawGizmo : MonoBehaviour
{
    //DRAW HANDLER
    [SerializeField] bool drawGizmo;
    [SerializeField] bool wireframe;
    [SerializeField] Color color = new Color(0, 1, 0, 1);
    [SerializeField] ColliderType colliderType;
    [SerializeField] SyncType syncType;
    //Box Param
    [SerializeField] Vector3 size;
    //Sphere and Capsule Param
    [SerializeField] float radius;
    //Capsule Param
    [SerializeField] float height;
    //TEXT HANDLER
    [SerializeField] bool showText = false;
    [SerializeField] string text;
    [SerializeField] int fontSize = 15;
    [SerializeField] Color fontColor = new Color(0, 0, 0, 1);
    [SerializeField] bool drawLine = false;
    [SerializeField] Color lineColor = new Color(0, 0, 0, 1);
    [SerializeField] Vector3 offset;
    //CONDITIONS
    [SerializeField] bool noBoxCollider;
    [SerializeField] bool noSphereCollider;
    [SerializeField] bool noCapsuleCollider;

    GUIStyle fontStyle = new GUIStyle();
    Vector3 textPos;
    Matrix4x4 rotationMatrix;
    BoxCollider boxCollider;
    SphereCollider sphereCollider;
    CapsuleCollider capsuleCollider;

    private void OnValidate()
    {
        CheckColliders();
    }
    public void CheckColliders()
    {
        //Check Box
        boxCollider = GetComponent<BoxCollider>();
        if(boxCollider == null)
            noBoxCollider = true;
        else
            noBoxCollider = false;
        //Check Sphere
        sphereCollider = GetComponent<SphereCollider>();
        if(sphereCollider == null)
            noSphereCollider = true;
        else
            noSphereCollider = false;
        //Check Capsule
        capsuleCollider = GetComponent<CapsuleCollider>();
        if(capsuleCollider == null)
            noCapsuleCollider = true;
        else
            noCapsuleCollider = false;
    }
    void ScynToObjectSize_Process()
    {
        size = transform.lossyScale;
        radius = 0.5f;
        rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, size);
        Gizmos.matrix = rotationMatrix;
    }
    void ScynToColliderSize_Process()
    {
        if(boxCollider != null)
        {
            size = GetComponent<BoxCollider>().size;
            rotationMatrix = Matrix4x4.TRS((transform.position - GetComponent<BoxCollider>().center), transform.rotation, size);
        }
        else if(sphereCollider != null)
        {
            radius = GetComponent<SphereCollider>().radius;
            rotationMatrix = Matrix4x4.TRS((transform.position - GetComponent<SphereCollider>().center), transform.rotation, size);
        }
        else if(capsuleCollider != null)
        {
            radius = GetComponent<CapsuleCollider>().radius;
            height = GetComponent<CapsuleCollider>().height;
            rotationMatrix = Matrix4x4.TRS((transform.position - GetComponent<CapsuleCollider>().center), transform.rotation, size);
        }
        Gizmos.matrix = rotationMatrix;
    }
    void Free_Process()
    {
        rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, size);
        Gizmos.matrix = rotationMatrix;
    }
    private void OnDrawGizmos()
    {
        if(!drawGizmo) return;

        Gizmos.color = color;

        switch(syncType)
        {
            case SyncType.ScynToObjectSize:
                ScynToObjectSize_Process();
                break;
            case SyncType.ScynToColliderSize:
                ScynToColliderSize_Process();
                break;
            case SyncType.Free:
                Free_Process();
                break;
            default:
                break;
        }
        switch(colliderType)
        {
            case ColliderType.Box:
                if(wireframe) Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                else Gizmos.DrawCube(Vector3.zero, Vector3.one);
                break;
            case ColliderType.Sphere:
                radius = Mathf.Clamp(radius, 0, Mathf.Infinity);
                if(wireframe) Gizmos.DrawWireSphere(Vector3.zero, radius);
                else Gizmos.DrawSphere(Vector3.zero, radius);
                break;
            case ColliderType.Capsule:
                height = Mathf.Clamp(height, 1, Mathf.Infinity);
                radius = Mathf.Clamp(radius, 0, Mathf.Infinity);
#if UNITY_EDITOR
                DrawWireCapsule(transform.position, transform.rotation, radius, height, color, wireframe);
#endif
                break;
            default:
                break;
        }

#if UNITY_EDITOR
        //Text Handler
        if(showText)
        {
            textPos = transform.position + offset;
            fontStyle.fontSize = fontSize;
            fontStyle.normal.textColor = fontColor;

            Handles.Label(textPos, text, fontStyle);

            if(drawLine)
            {
                Handles.color = lineColor;
                Handles.DrawLine(textPos, transform.position);
            }
        }
#endif
    }
#if UNITY_EDITOR
    //Helper Method
    public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color), bool _wireFrame = true)
    {
        if(_color != default(Color))
            Handles.color = _color;
        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
        using(new Handles.DrawingScope(angleMatrix))
        {
            var pointOffset = (_height - (_radius * 2)) / 2;

            //draw sideways
            if(_wireFrame)
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
            else
                Handles.DrawSolidArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
            Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
            Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
            if(_wireFrame)
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
            else
                Handles.DrawSolidArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
            //draw frontways
            if(_wireFrame)
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
            else
                Handles.DrawSolidArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
            Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
            Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
            if(_wireFrame)
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
            else
                Handles.DrawSolidArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
            //draw center
            if(_wireFrame)
            {
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);
            }
            else
            {
                Handles.DrawSolidDisc(Vector3.up * pointOffset, Vector3.up, _radius);
                Handles.DrawSolidDisc(Vector3.down * pointOffset, Vector3.up, _radius);
            }
        }
    }
#endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(DrawGizmo))]
public class DrawGizmoEditor : Editor
{
    DrawGizmo script;
    SerializedObject s_Script;
    //Draw Handler
    SerializedProperty s_DrawGizmo;//bool
    SerializedProperty s_Wireframe;//bool
    SerializedProperty s_Color;//color
    SerializedProperty s_ColliderType;//enum
    SerializedProperty s_SyncType;//enum
    SerializedProperty s_Size;//vector3
    SerializedProperty s_Radius;//float
    SerializedProperty s_Height;//float
    //Text Handler
    SerializedProperty s_ShowText;//bool
    SerializedProperty s_Text;//string
    SerializedProperty s_FontSize;//float
    SerializedProperty s_FontColor;//color
    SerializedProperty s_DrawLine;//bool
    SerializedProperty s_LineColor;//color
    SerializedProperty s_Offset;//vector3
    //Conditions
    SerializedProperty s_NoBoxColl;//bool
    SerializedProperty s_NoSphereColl;//bool
    SerializedProperty s_NoCapsuleColl;//bool

    GUIContent singleGuiContent = new GUIContent();

    private void OnEnable()
    {
        EditorApplication.hierarchyChanged += ChangedInspector;
        //Target
        script = (DrawGizmo)target;
        //SerializedObject
        s_Script = new SerializedObject(script);
        //SerializedPropertys
        s_DrawGizmo     = s_Script.FindProperty("drawGizmo");
        s_Wireframe     = s_Script.FindProperty("wireframe");
        s_Color         = s_Script.FindProperty("color");
        s_ColliderType  = s_Script.FindProperty("colliderType");
        s_SyncType      = s_Script.FindProperty("syncType");
        s_Size          = s_Script.FindProperty("size");
        s_Radius        = s_Script.FindProperty("radius");
        s_Height        = s_Script.FindProperty("height");
        s_ShowText      = s_Script.FindProperty("showText");
        s_Text          = s_Script.FindProperty("text");
        s_FontSize      = s_Script.FindProperty("fontSize");
        s_FontColor     = s_Script.FindProperty("fontColor");
        s_DrawLine      = s_Script.FindProperty("drawLine");
        s_LineColor     = s_Script.FindProperty("lineColor");
        s_Offset        = s_Script.FindProperty("offset");
        s_NoBoxColl     = s_Script.FindProperty("noBoxCollider");
        s_NoSphereColl  = s_Script.FindProperty("noSphereCollider");
        s_NoCapsuleColl = s_Script.FindProperty("noCapsuleCollider");
    }
    public override void OnInspectorGUI()
    {
        s_Script.Update();

        #region Draw Handler
        EditorGUILayout.BeginVertical("Box");//VER3

        EditorGUILayout.BeginHorizontal();//HOR1
        EditorGUIUtility.labelWidth = 80;
        singleGuiContent.text = "Draw Gizmo";
        EditorGUILayout.PropertyField(s_DrawGizmo, singleGuiContent, GUILayout.Width(120));

        singleGuiContent.text = "Wireframe";
        EditorGUILayout.PropertyField(s_Wireframe, singleGuiContent);
        EditorGUIUtility.labelWidth = 167;
        EditorGUILayout.EndHorizontal();//HOR1

        singleGuiContent.text = "Gizmo Type";
        EditorGUILayout.PropertyField(s_ColliderType, singleGuiContent);

        singleGuiContent.text = "Sync Type";
        EditorGUILayout.PropertyField(s_SyncType, singleGuiContent);

        singleGuiContent.text = "Color";
        EditorGUILayout.PropertyField(s_Color, singleGuiContent,GUILayout.Width(250));

        if(s_ColliderType.enumValueIndex == (int)ColliderType.Sphere || s_ColliderType.enumValueIndex == (int)ColliderType.Capsule)
        {
            if(s_SyncType.enumValueIndex == (int)SyncType.Free)
            {
                singleGuiContent.text = "Radius";
                EditorGUILayout.PropertyField(s_Radius, singleGuiContent);
            }
            else if(s_SyncType.enumValueIndex == (int)SyncType.ScynToColliderSize)
            {
                if(s_NoSphereColl.boolValue == true)
                {
                    EditorGUILayout.BeginHorizontal();//HOR2
                    EditorGUILayout.HelpBox("Please add a Sphere Collider", MessageType.Error);
                    if(GUILayout.Button("Add", GUILayout.Height(38)))
                    {
                        script.gameObject.AddComponent<SphereCollider>();
                        script.CheckColliders();
                    }
                    EditorGUILayout.EndHorizontal();//HOR2
                }
            }

            if(s_ColliderType.enumValueIndex == (int)ColliderType.Capsule)
            {
                if(s_SyncType.enumValueIndex == (int)SyncType.Free)
                {
                    singleGuiContent.text = "Height";
                    EditorGUILayout.PropertyField(s_Height, singleGuiContent);
                }
                else if(s_SyncType.enumValueIndex == (int)SyncType.ScynToColliderSize)
                {
                    if(s_NoCapsuleColl.boolValue == true)
                    {
                        EditorGUILayout.BeginHorizontal();//HOR3
                        EditorGUILayout.HelpBox("Please add a Capsule Collider", MessageType.Error);
                        if(GUILayout.Button("Add", GUILayout.Height(38)))
                        {
                            script.gameObject.AddComponent<CapsuleCollider>();
                            script.CheckColliders();
                        }
                        EditorGUILayout.EndHorizontal();//HOR3
                    }
                }
            }
        }
        else if(s_ColliderType.enumValueIndex == (int)ColliderType.Box)
        {
            if(s_SyncType.enumValueIndex == (int)SyncType.Free)
            {
                singleGuiContent.text = "Size";
                EditorGUILayout.PropertyField(s_Size, singleGuiContent);
            }
            else if(s_SyncType.enumValueIndex == (int)SyncType.ScynToColliderSize)
            {
                if(s_NoBoxColl.boolValue == true)
                {
                    EditorGUILayout.BeginHorizontal();//HOR4
                    EditorGUILayout.HelpBox("Please add a Box Collider", MessageType.Error);
                    if(GUILayout.Button("Add", GUILayout.Height(38)))
                    {
                        script.gameObject.AddComponent<BoxCollider>();
                        script.CheckColliders();
                    }
                    EditorGUILayout.EndHorizontal();//HOR4
                }
            }
        }

        EditorGUILayout.EndVertical();//VER3
        #endregion

        #region Text Handler
        singleGuiContent.text = "Show Text";
        EditorGUILayout.PropertyField(s_ShowText);
        if(s_ShowText.boolValue)
        {
            EditorGUILayout.BeginHorizontal("Box");//HOR5

            EditorGUILayout.BeginVertical();//VER2
            EditorGUILayout.LabelField("Text",EditorStyles.boldLabel, GUILayout.Width(50));
            s_Text.stringValue = EditorGUILayout.TextArea(s_Text.stringValue,GUILayout.Height(100), GUILayout.Width(150));
            EditorGUILayout.EndVertical();//VER2

            EditorGUILayout.BeginVertical();//VER1
            EditorGUIUtility.labelWidth = 80;
            GUILayout.Space(2);
            singleGuiContent.text = "Font Size";
            EditorGUILayout.PropertyField(s_FontSize,GUILayout.Width(150));

            GUILayout.Space(5);

            singleGuiContent.text = "Font Color";
            EditorGUILayout.PropertyField(s_FontColor, GUILayout.Width(200));

            GUILayout.Space(5);

            singleGuiContent.text = "Draw Line";
            EditorGUILayout.PropertyField(s_DrawLine, GUILayout.Width(200));

            GUILayout.Space(5);

            if(s_DrawLine.boolValue)
            {
                singleGuiContent.text = "Line Color";
                EditorGUILayout.PropertyField(s_LineColor, GUILayout.Width(200));

                GUILayout.Space(5);

                singleGuiContent.text = "Offset";
                EditorGUILayout.PropertyField(s_Offset, GUILayout.Width(200));
            }

            EditorGUIUtility.labelWidth = 167;
            EditorGUILayout.EndVertical();//VER1

            EditorGUILayout.EndHorizontal();//HOR5
        }
        #endregion

        s_Script.ApplyModifiedProperties();
    }
    void ChangedInspector()
    {
        if(script != null)
            script.CheckColliders();
    }
}
#endif
