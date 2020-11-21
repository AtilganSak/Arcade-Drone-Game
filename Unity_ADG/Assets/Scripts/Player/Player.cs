using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int _money;
    public int money { get => _money; private set => _money = value; }

    [SerializeField] bool _saveMoney;
    public bool saveMoney { get => _saveMoney; }

    [SerializeField] bool _isAlive;
    public bool isAlive { get => _isAlive; private set => _isAlive = value; }

    public Action OnDead;

    PlayerUI playerUI;

    const string PLAYER_MONEY = "PlayerMoney";

    private void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
        if (playerUI == null) enabled = false;

        if (saveMoney)
        {
            if (PlayerPrefs.HasKey(PLAYER_MONEY))
                money = PlayerPrefs.GetInt(PLAYER_MONEY);
        }

        UpdateUI();

        isAlive = true;
    }
    public void EarnMoney(int amount)
    {
        money += amount;

        if(saveMoney)
            PlayerPrefs.SetInt(PLAYER_MONEY, money);

        UpdateUI();
    }
    public void Dead()
    {
        isAlive = false;

        OnDead();
    }
    void UpdateUI()
    {
        playerUI.UpdateMoneyText(money);
    }
}
