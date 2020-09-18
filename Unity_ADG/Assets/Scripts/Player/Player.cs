using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public int money;

    PlayerUI playerUI;

    const string PLAYER_MONEY = "PlayerMoney";

    private void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
        if (playerUI == null) enabled = false;

        if (PlayerPrefs.HasKey(PLAYER_MONEY))
            money = PlayerPrefs.GetInt(PLAYER_MONEY);

        UpdateUI();
    }
    public void EarnMoney(int amount)
    {
        money += amount;

        PlayerPrefs.SetInt(PLAYER_MONEY, money);

        UpdateUI();
    }
    void UpdateUI()
    {
        playerUI.UpdateMoneyText(money);
    }
}
