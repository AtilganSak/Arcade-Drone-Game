using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCanvas : MonoBehaviour
{
    UIManager uIManager;
    GameManager gameManager;

    public GameObject entryPanel;
    public GameObject endPanel;

    private void OnEnable()
    {
        uIManager = FindObjectOfType<UIManager>();
        gameManager = FindObjectOfType<GameManager>();
    }
    private void Start()
    {
        if (!gameManager.noEntry)
            entryPanel.SetActive(true);

        HideEndPanel();
    }
    public void ShowEndPanel()
    {
        endPanel.SetActive(true);
    }
    public void HideEndPanel()
    {
        endPanel.SetActive(false);
    }
    public void Pressed_RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Pressed_MenuButton()
    {
        Debug.LogWarning("Soon!");
    }
    public void Pressed_StartButton()
    {
        uIManager.Pressed_StartButton();

        entryPanel.SetActive(false);
    }
}
