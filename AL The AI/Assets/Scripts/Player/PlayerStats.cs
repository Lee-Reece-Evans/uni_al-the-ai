using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public int money;
    public int initialMoney = 500;

    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }

    void Start()
    {
        money = initialMoney;
        UpdateMoneyText();
    }

    private void Die()
    {
        // 
    }

    public void Respawn()
    {
        //
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyText();
    }

    public void RemoveMoney(int amount)
    {
        money -= amount;
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        OnScreenUI_Manager.Instance.SetMoneyText(money);
    }
}
