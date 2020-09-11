using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameCanvas gameCanvas { get; private set; }
    GameManager gameManager;

    private void OnEnable()
    {
        gameCanvas = FindObjectOfType<GameCanvas>();
        gameManager = FindObjectOfType<GameManager>();
    }
    public void Pressed_StartButton()
    {
        gameManager.StartGame();
    }
}
