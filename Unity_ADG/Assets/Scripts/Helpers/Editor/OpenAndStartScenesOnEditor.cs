using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine;

public class OpenAndStartScenesOnEditor
{
    static string dataPath = Application.dataPath;

    ////START SCENES
    //[MenuItem("Tools/Start Scene/Constructor Scene")]
    //public static void StartWithTutorialScene() =>              StartGame("ConstructorScene");

    //OPEN SCENES
    [MenuItem("ATL/Open Scene/Test Scene")]
    public static void OpenTestScene() => OpenScene("Drone Test");
    [MenuItem("ATL/Open Scene/Demo Test Scene")]
    public static void OpenDemoTestScene() => OpenScene("Demo_Test_Scene");
    [MenuItem("ATL/Open Scene/Gameplay Test")]
    public static void OpenGameplayTestScene() => OpenScene("GamePlayTestScene");
    [MenuItem("ATL/Open Scene/City")]
    public static void OpenCityScene() => OpenScene("City");
    [MenuItem("ATL/Open Scene/PathTestScene")]
    public static void OpenPathTestScene() => OpenScene("PathTestScene");

    //FUNCTIONS
    private static bool OpenScene(string scene)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {            
            EditorSceneManager.OpenScene("Assets/Scenes/" + scene + ".unity");
            return true;
        }
        else
            return false;
    }
    public static void StartGame(string name)
    {
        if(OpenScene(name))
            EditorApplication.isPlaying = true;
    }
}
