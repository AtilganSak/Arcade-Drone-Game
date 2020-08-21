using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public bool async;

    public void LoadScene(int id)
    {
        if (!async)
            SceneManager.LoadScene(id);
        else
            SceneManager.LoadSceneAsync(id);
    }
    public void LoadScene(string name)
    {
        if (!async)
            SceneManager.LoadScene(name);
        else
            SceneManager.LoadSceneAsync(name);
    }
}
