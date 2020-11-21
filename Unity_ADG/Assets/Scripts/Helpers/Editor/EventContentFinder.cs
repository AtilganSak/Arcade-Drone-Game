using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EventFinder
{
    public class EventContentFinder : EditorWindow
    {
        [MenuItem("ATL/Event Content Finder")]
        static void Init()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(EventContentFinder));
            window.title = "Event Content Finder";
            window.Show();
        }

        MonoBehaviour firstMono;
        GameObject firstObject;

        bool recursive;

        string methodName;
        string gameObjectName;

        string[] searchStates;

        int selectedSearchStateIndex;

        const byte emitMessageLimit = 6;
        const string RECURSIVE = "recursive";

        List<GameObject> foundObjects = new List<GameObject>();
        List<EditorMessage> editorMessages = new List<EditorMessage>();

        private void OnEnable()
        {
            methodName = "";
            gameObjectName = "";

            searchStates = new string[2];
            searchStates[0] = "Method Name";
            searchStates[1] = "Object Name";

            if (PlayerPrefs.HasKey(RECURSIVE))
                recursive = PlayerPrefs.GetInt(RECURSIVE) == 0 ? false : true;

            EditorSceneManager.activeSceneChangedInEditMode -= ChangedScene;

            EditorSceneManager.activeSceneChangedInEditMode += ChangedScene;
        }
        private void OnLostFocus()
        {
            PlayerPrefs.SetInt(RECURSIVE, recursive ? 1 : 0);
        }
        void ChangedScene(Scene scene, Scene scene1)
        {
            foundObjects.Clear();
        }
        private void OnGUI()
        {
            selectedSearchStateIndex = EditorGUILayout.Popup("Search Target", selectedSearchStateIndex, searchStates);

            if (selectedSearchStateIndex == 0)
            {
                methodName = EditorGUILayout.TextField("Method Name", methodName);
            }
            else if (selectedSearchStateIndex == 1)
            {
                gameObjectName = EditorGUILayout.TextField("Object Name", gameObjectName);
            }

            recursive = EditorGUILayout.Toggle("Recursive", recursive);

            if (GUILayout.Button("Clear"))
            {
                foundObjects.Clear();
                methodName = "";
                gameObjectName = "";

                GUI.FocusControl("");
            }
            if (GUILayout.Button("Play"))
            {
                Play();
            }

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                for (int i = 0; i < foundObjects.Count; i++)
                {
                    if (GUILayout.Button(foundObjects[i].name))
                    {
                        Selection.activeObject = foundObjects[i];
                        EditorGUIUtility.PingObject(foundObjects[i]);
                    }
                }
            }

            MessageHandler();
        }
        void Play()
        {
            if (!Validate()) return;

            foundObjects.Clear();

            FindMonoBehaviours();

            if (foundObjects.Count == 0)
            {
                EmitMessage("Not found any objects.", MessageType.Info);
            }
            else
            {
                editorMessages.Clear();
            }
        }
        void FindMonoBehaviours()
        {
            MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();
            foreach (var item in monoBehaviours)
            {
                firstMono = item;
                firstObject = firstMono.gameObject;
                FieldInfo[] fieldInfos = item.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                DetectFields(fieldInfos, item);
            }
        }
        void DetectFields(FieldInfo[] fieldInfos, object obj)
        {
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (obj != null)
                {
                    if (!fieldInfos[i].IsStatic)
                    {
                        if (!DetectUnityEvent(fieldInfos[i], obj))
                        {
                            Type tp = fieldInfos[i].FieldType;
                            if (tp.IsValueType || tp.IsClass)
                            {
                                if (typeof(IList).IsAssignableFrom(tp))
                                {
                                    IList genList = (IList)fieldInfos[i].GetValue(obj);
                                    if (genList != null)
                                    {
                                        for (int l = 0; l < genList.Count; l++)
                                        {
                                            object objs = genList[l];
                                            if (objs != null)
                                            {
                                                Type tp1 = genList[l].GetType();
                                                FieldInfo[] fieldInfos1 = tp1.GetFields();
                                                if (!recursive)
                                                {
                                                    for (int f = 0; f < fieldInfos1.Length; f++)
                                                    {
                                                        if (!fieldInfos1[f].IsStatic)
                                                        {
                                                            DetectUnityEvent(fieldInfos1[f], objs);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    DetectFields(fieldInfos1, objs);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    FieldInfo[] fieldInfos1 = tp.GetFields();
                                    object objs = fieldInfos[i].GetValue(obj);
                                    if (!recursive)
                                    {
                                        for (int f = 0; f < fieldInfos1.Length; f++)
                                        {
                                            if (objs != null)
                                            {
                                                if (!fieldInfos1[f].IsStatic)
                                                {
                                                    DetectUnityEvent(fieldInfos1[f], objs);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        DetectFields(fieldInfos1, objs);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        bool DetectUnityEvent(FieldInfo fieldInfo, object obj)
        {
            if (fieldInfo.FieldType == typeof(UnityEvent))
            {
                UnityEvent tempUE = (UnityEvent)fieldInfo.GetValue(obj);
                FindObject(tempUE);

                return true;
            }
            else if (fieldInfo.FieldType == typeof(UnityEvent[]))
            {
                UnityEvent[] tempUE = (UnityEvent[])fieldInfo.GetValue(obj);
                if (tempUE != null)
                {
                    for (int l = 0; l < tempUE.Length; l++)
                    {
                        FindObject(tempUE[l]);
                    }

                    return true;
                }
            }
            else if (fieldInfo.FieldType == typeof(List<UnityEvent>))
            {
                List<UnityEvent> tempUE = (List<UnityEvent>)fieldInfo.GetValue(obj);
                if (tempUE != null)
                {
                    for (int l = 0; l < tempUE.Count; l++)
                    {
                        FindObject(tempUE[l]);
                    }

                    return true;
                }
            }

            return false;
        }
        void FindObject(UnityEvent unityEvent)
        {
            if (unityEvent == null) return;

            for (int k = 0; k < unityEvent.GetPersistentEventCount(); k++)
            {
                if (selectedSearchStateIndex == 0)
                {
                    if (unityEvent.GetPersistentMethodName(k) == methodName)
                    {
                        foundObjects.Add(firstObject);
                    }
                }
                else if (selectedSearchStateIndex == 1)
                {
                    if (unityEvent.GetPersistentTarget(k).name == gameObjectName)
                    {
                        foundObjects.Add(firstObject);
                    }
                }
            }
        }
        bool Validate()
        {
            if (selectedSearchStateIndex == 0)
            {
                if (methodName == "")
                {
                    EmitMessage("Please enter the Method Name!", MessageType.Warning);
                    return false;
                }
            }
            else if (selectedSearchStateIndex == 1)
            {
                if (gameObjectName == "")
                {
                    EmitMessage("Please enter the Object Name!", MessageType.Warning);
                    return false;
                }
            }
            return true;
        }
        void EmitMessage(string _message, MessageType _messageType = MessageType.None, float _duringTime = 3)
        {
            if (editorMessages.Count < emitMessageLimit)
            {
                editorMessages.Add(
                    new EditorMessage
                    {
                        Message = _message,
                        MessageType = _messageType,
                        ShowDuring = _duringTime,
                        timer = (float)EditorApplication.timeSinceStartup
                    }
                );
            }
        }
        private void MessageHandler()
        {
            for (int i = 0; i < editorMessages.Count; i++)
            {
                EditorGUILayout.HelpBox(editorMessages[i].Message, editorMessages[i].MessageType);
                if (EditorApplication.timeSinceStartup - editorMessages[i].timer > editorMessages[i].ShowDuring)
                {
                    editorMessages.RemoveAt(i);
                }
            }
            Repaint();
        }
    }

    [System.Serializable]
    public struct EditorMessage
    {
        public string Message;
        public MessageType MessageType;
        public float ShowDuring;
        public float timer { get; set; }
    }
}