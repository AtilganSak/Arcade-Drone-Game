using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class RoadPath : MonoBehaviour
{
    public bool drawPath = true;
    public bool lockPath;
    public float size = 0.3F;
    public Color pathLineColor = Color.white;
    public Color pathPointColor = Color.white;
    [HideInInspector] public Point[] points;

    Transform c_Transform;

    private void OnEnable()
    {
        c_Transform = transform;
    }
    public Vector3 GetPoint(int index)
    {
        return c_Transform.TransformPoint(points[index].position);
    }
#if UNITY_EDITOR
    public Vector3 GetPointForEditor(int index)
    {
        return transform.TransformPoint(points[index].position);
    }
#endif
    public void Reset()
    {
        points = new RoadPath.Point[1]
            {
                new RoadPath.Point
                {
                    position = Vector3.zero
                }
            };
    }

    [Serializable]
    public struct Point
    {
        public Vector3 position;
        public int nextPointIndex;
        public int previousPointIndex;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GameObject go = Selection.activeGameObject;
        if (go != gameObject)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 p1 = GetPointForEditor(i);
                Vector3 p2 = GetPointForEditor(i);

                Handles.color = pathPointColor;
                Handles.SphereHandleCap(-1, p1, Quaternion.identity, size, EventType.Repaint);

                if (i + 1 > points.Length - 1)
                {
                    if (lockPath)
                        p2 = GetPointForEditor(0);
                }
                else
                {
                    p2 = GetPointForEditor(i + 1);
                }
                Handles.color = pathLineColor;
                Handles.DrawLine(p1, p2);
            }
        }
    }
#endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(RoadPath))]
public class RoadPathEditor : Editor
{
    RoadPath script { get => target as RoadPath; }

    int selectedPointIndex = -1;
    int c_selectedPointIndex;

    bool deletedComponent;

    private void OnEnable()
    {
        Tools.hidden = true;

        if (script.points == null || script.points.Length == 0)
        {
            script.points = new RoadPath.Point[1]
            {
                new RoadPath.Point
                {
                    position = Vector3.zero
                }
            };
        }
    }
    private void OnDisable()
    {
        Tools.hidden = false;
        selectedPointIndex = -1;
    }
    public override void OnInspectorGUI()
    {
        if (deletedComponent) return;

        EditorGUI.BeginChangeCheck();
        GUI.backgroundColor = Color.cyan;
        using (new EditorGUILayout.VerticalScope("Box"))
        {
            EditorGUILayout.LabelField("Point Settings");
            EditorGUI.indentLevel++;
            if (selectedPointIndex == -1)
            {
                GUI.contentColor = Color.red;
                EditorGUILayout.LabelField("Nothing Selected Any Point!");
                GUI.contentColor = Color.white;
            }
            else
            {
                Vector3 pointPos = script.GetPointForEditor(selectedPointIndex);
                EditorGUILayout.LabelField("Index: " + selectedPointIndex);
                EditorGUI.BeginChangeCheck();
                pointPos = EditorGUILayout.Vector3Field("Position", pointPos);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(script, "Changed Point Position");
                    EditorUtility.SetDirty(script);
                    script.points[selectedPointIndex].position = script.transform.InverseTransformPoint(pointPos);
                }
            }
            EditorGUI.indentLevel--;
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(script);
        }

        using (new EditorGUILayout.VerticalScope("Box"))
        {
            EditorGUILayout.LabelField("PathSettings", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUIUtility.labelWidth = 70;
                script.drawPath = EditorGUILayout.Toggle("Draw Path", script.drawPath, GUILayout.Width(100));
                GUILayout.Space(50);
                script.lockPath = EditorGUILayout.Toggle("Lock Path", script.lockPath);
            }
            script.size = EditorGUILayout.FloatField("Point Size", script.size);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUIUtility.labelWidth = 70;
                script.pathLineColor = EditorGUILayout.ColorField("Line Color", script.pathLineColor, GUILayout.Width(150));
                GUILayout.Space(50);
                EditorGUIUtility.labelWidth = 70;
                script.pathPointColor = EditorGUILayout.ColorField("Point Color", script.pathPointColor, GUILayout.Width(150));
                EditorGUIUtility.labelWidth = 140.9F;
            }
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(script);

            if (!Tools.hidden)
                GUI.backgroundColor = Color.green;
            else
                GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Move Path"))
            {
                Tools.hidden = !Tools.hidden;
                if (Tools.hidden)
                {
                    selectedPointIndex = c_selectedPointIndex;
                }
                else
                {
                    c_selectedPointIndex = selectedPointIndex;
                    selectedPointIndex = -1;
                }
                SceneView.RepaintAll();
            }
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Reset Path"))
            {
                script.Reset();
                EditorUtility.SetDirty(script);
            }
        }
    }
    private void OnSceneGUI()
    {
        if (deletedComponent) return;

        DrawPath();
    }
    void DrawPath()
    {
        if (script.drawPath)
        {
            if (Event.current != null)
            {
                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
                {
                    if (selectedPointIndex != -1)
                    {
                        if (script.points.Length > 1)
                        {
                            List<RoadPath.Point> tmp = script.points.ToList();
                            tmp.RemoveAt(selectedPointIndex);
                            script.points = tmp.ToArray();
                            selectedPointIndex = -1;
                            Event.current.Use();
                        }
                        else
                        {
                            if (EditorUtility.DisplayDialog("Delete Path", "Do you want to delete Path?", "Yes", "No"))
                            {
                                DestroyImmediate(script.gameObject.GetComponent<RoadPath>());
                                deletedComponent = true;
                                Event.current.Use();
                                EditorGUIUtility.ExitGUI();
                                return;
                            }
                        }
                    }
                    Event.current.Use();
                }
            }
            Handles.matrix = script.transform.localToWorldMatrix;
            for (int i = 0; i < script.points.Length; i++)
            {
                Vector3 firstPoint = script.points[i].position;
                Vector3 secondPoint = Vector3.zero;
                if (i + 1 < script.points.Length)
                {
                    secondPoint = script.points[i + 1].position;
                }
                else
                {
                    if (script.lockPath)
                        secondPoint = script.points[0].position;
                }
                if (script.points.Length > 1 && (script.lockPath || i != script.points.Length - 1))
                {
                    Handles.color = script.pathLineColor;
                    Handles.DrawLine(firstPoint, secondPoint);
                    Handles.color = Color.white;
                }
                if (i == selectedPointIndex)
                {
                    Vector3 point = script.points[i].position;
                    EditorGUI.BeginChangeCheck();
                    point = Handles.PositionHandle(point, Quaternion.identity);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(script, "Move Point");
                        EditorUtility.SetDirty(script);
                        script.points[i].position = point/*script.transform.InverseTransformPoint(point)*/;
                    }
                }
                else
                {
                    Handles.color = script.pathPointColor;
                    if (Handles.Button(script.points[i].position, Quaternion.identity, script.size, script.size, Handles.SphereHandleCap))
                    {
                        selectedPointIndex = i;
                        Repaint();
                    }
                    Handles.color = Color.white;
                }
                Handles.color = Color.green;
                if (!script.lockPath || script.points.Length == 2)
                {
                    if (i == script.points.Length - 1)
                    {
                        Vector3 direction = Vector3.forward;
                        if (script.points.Length > 1)
                            direction = script.points[0].position - script.points[1].position;
                        Vector3 firstButtonPos = script.points[0].position + direction.normalized * (script.size + 1);
                        if (Handles.Button(firstButtonPos/*script.transform.TransformPoint(firstButtonPos)*/, Quaternion.identity, script.size, script.size, Handles.SphereHandleCap))
                        {
                            AddPointToFirst(firstButtonPos);
                        }
                    }
                    else if (i == 0)
                    {
                        Vector3 direction = script.points[script.points.Length - 1].position - script.points[script.points.Length - 2].position;
                        Vector3 lastButtonPos = script.points[script.points.Length - 1].position + direction.normalized * (script.size + 1);
                        if (Handles.Button(lastButtonPos/*script.transform.TransformPoint(lastButtonPos)*/, Quaternion.identity, script.size, script.size, Handles.SphereHandleCap))
                        {
                            AddPointToEnd(lastButtonPos);
                        }
                    }
                }
                Handles.color = Color.white;
            }
        }
    }
    void AddPointToFirst(Vector3 firstButtonPos)
    {
        Undo.RecordObject(script, "Addet Point to First");
        List<RoadPath.Point> tmpList = script.points.ToList();
        RoadPath.Point newPoint = new RoadPath.Point();
        tmpList.Insert(0, new RoadPath.Point { position = firstButtonPos });
        script.points = tmpList.ToArray();
        if (selectedPointIndex != -1)
            selectedPointIndex = 0;
    }
    void AddPointToEnd(Vector3 lastButtonPos)
    {
        Undo.RecordObject(script, "Addet Point to End");
        List<RoadPath.Point> tmpList = script.points.ToList();
        tmpList.Add(new RoadPath.Point { position = lastButtonPos });
        script.points = tmpList.ToArray();
        if (selectedPointIndex != -1)
            selectedPointIndex = script.points.Length - 1;
    }
}
#endif
