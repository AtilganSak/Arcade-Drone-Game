using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;
using UnityEngine.Events;

public class DOSequence : MonoBehaviour
{
    public List<DOBase> dos = new List<DOBase>();

    public bool doOnStart;
    public bool updateLoop;

    public UnityEvent onCompletedSequence;    
    private void OnEnable()
    {
        for (int i = 0; i < dos.Count; i++)
        {
            if (i + 1 < dos.Count)
            {
                if (dos[i + 1].connect)
                    dos[i].DOComplete.AddListener(dos[i + 1].DO);
            }
            if(i == dos.Count - 1)
                dos[i].DOComplete.AddListener(CompletedSequence);
        }
    }
    private void OnDestroy()
    {
        for (int i = 0; i < dos.Count; i++)
        {
            if (i + 1 < dos.Count)
                dos[i].DOComplete.RemoveAllListeners();
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < dos.Count; i++)
        {
            if (i + 1 < dos.Count)
                dos[i].DOComplete.RemoveAllListeners();
        }
    }
    private void Start()
    {
        if (doOnStart)
            DO();
    }
    private void Update()
    {
        if (updateLoop)
        {
            DO();
        }
    }
    public void DO()
    {
        for (int i = 0; i < dos.Count; i++)
        {
            if (!dos[i].connect)
                dos[i].DO();
        }
    }
    public void ResetAll()
    {
        for (int i = 0; i < dos.Count; i++)
        {
            dos[i].ResetDO();
        }
    }
    private void CompletedSequence()
    {
        onCompletedSequence.Invoke();
    }
    private void OnValidate()
    {
        MakeAsync();
    }
    public void MakeAsync()
    {
        for (int i = 0; i < dos.Count; i++)
        {
            if(dos[i] != null)
                dos[i].orderIndex = i;
        }
        dos.OrderBy(x => x.orderIndex);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(DOSequence))]
public class DOSequenceEditor : Editor
{
    DOSequence script { get => target as DOSequence; }

    SerializedObject s_Script;
    SerializedProperty p_DoOnStart;
    SerializedProperty p_UpdateLoop;
    SerializedProperty p_OnCompletedSequence;

    ReorderableList reorderableList;

    private void OnEnable()
    {
        s_Script = new SerializedObject(script);
        p_DoOnStart = s_Script.FindProperty("doOnStart");
        p_UpdateLoop = s_Script.FindProperty("updateLoop");
        p_OnCompletedSequence = s_Script.FindProperty("onCompletedSequence");

        reorderableList = new ReorderableList(script.dos, typeof(DOBase), true, true, true, true);

        reorderableList.onChangedCallback += OnChangedItem;
        reorderableList.drawHeaderCallback += DrawHeader;
        reorderableList.drawElementCallback += DrawElement;
        reorderableList.onAddCallback += AddItem;
        reorderableList.onRemoveCallback += RemoveItem;
    }
    private void OnDestroy()
    {
        reorderableList.onChangedCallback -= OnChangedItem;
        reorderableList.drawHeaderCallback -= DrawHeader;
        reorderableList.drawElementCallback -= DrawElement;
        reorderableList.onAddCallback -= AddItem;
        reorderableList.onRemoveCallback -= RemoveItem;
    }
    private void DrawHeader(Rect rect)
    {
        GUI.Label(rect, "DO List");
    }
    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        script.dos[index] = (DOBase)EditorGUI.ObjectField(new Rect(rect.x + 30, rect.y, rect.width - 30, rect.height), script.dos[index], typeof(DOBase));
        if (script.dos[index] != null)
        {
            script.dos[index].connect = EditorGUI.Toggle(new Rect(rect.x, rect.y, 25, 25), script.dos[index].connect);
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(script);
        EditorGUILayout.EndHorizontal();
    }
    private void OnChangedItem(ReorderableList list)
    {
        script.MakeAsync();
        EditorUtility.SetDirty(script);
    }
    private void AddItem(ReorderableList list)
    {
        script.dos.Add(new DOBase());

        EditorUtility.SetDirty(script);
    }
    private void RemoveItem(ReorderableList list)
    {
        script.dos.RemoveAt(list.index);

        EditorUtility.SetDirty(script);
    }
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        if (EditorApplication.isPlaying)
        {
            reorderableList.draggable = false;
        }
        else
        {
            reorderableList.draggable = true;
        }
        s_Script.Update();
        EditorGUILayout.PropertyField(p_DoOnStart);
        EditorGUILayout.PropertyField(p_UpdateLoop);
        EditorGUILayout.PropertyField(p_OnCompletedSequence);

        reorderableList.DoLayoutList();
        s_Script.ApplyModifiedProperties();
    }
}
#endif
