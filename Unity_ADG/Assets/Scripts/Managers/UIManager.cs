using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameCanvas gameCanvas { get; private set; }
    GameManager gameManager;
    DamageUI damageUI;

    private void OnEnable()
    {
        gameCanvas = FindObjectOfType<GameCanvas>();
        damageUI = FindObjectOfType<DamageUI>();
        gameManager = FindObjectOfType<GameManager>();
    }
    public void Pressed_StartButton()
    {
        gameManager.StartGame();
    }
    public void ClearEffects()
    {
        damageUI.HideDamageScreenEffect();
        damageUI.HideDamageText();
    }
}
