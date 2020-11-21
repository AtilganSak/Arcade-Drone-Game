using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("ATL/Missing Finder")]
    static void Init()
    {
        EditorWindow.GetWindow<FindMissingScripts>().Show();
    }

    List<GameObject> foundObjects = new List<GameObject>();

    private void OnGUI()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length > 0)
        {
        }
        if (GUILayout.Button("Find"))
        {
            Find();
        }

        //if (GUILayout.Button("Find In Scene"))
        //{
        //    FindScene();
        //}

        ListiningsMissingObject();
    }

    void FindScene()
    {

    }
    void Find()
    {
        foundObjects.Clear();
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        for (int i = 0; i < gameObjects.Length; i++)
        {
            Component[] components = (Component[])gameObjects[i].GetComponents<Component>();
            if (components != null && components.Length > 0)
            {
                for (int k = 0; k < components.Length; k++)
                {
                    if (components[k] == null)
                    {
                        foundObjects.Add(gameObjects[i]);
                        continue;
                    }
                }
            }
        }
    }
    void ListiningsMissingObject()
    {
        if (foundObjects.Count > 0)
        {
            EditorGUILayout.BeginVertical("Box");
            for (int i = 0; i < foundObjects.Count; i++)
            {
                if (GUILayout.Button(foundObjects[i].name))
                {
                    Selection.activeGameObject = foundObjects[i];
                    EditorGUIUtility.PingObject(foundObjects[i]);
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
