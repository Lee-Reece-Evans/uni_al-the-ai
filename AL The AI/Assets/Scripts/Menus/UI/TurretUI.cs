using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurretUI : MonoBehaviour
{
    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI ammoOne;
    [SerializeField] private TextMeshProUGUI ammoTwo;
    [SerializeField] private TextMeshProUGUI shieldCost;
    [SerializeField] private TextMeshProUGUI upgradeCost;
    [SerializeField] private TextMeshProUGUI refundCost;

    [Header("Button Components")]
    [SerializeField] private Button shieldButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button ammoOneButton;
    [SerializeField] private Button ammoTwoButton;

    private Turret_Base turret;
    private IngameMenuManager UI;

    public void SetTurret(Turret_Base _turret)
    {
        if (UI == null)
            UI = IngameMenuManager.instance;

        if (UI.turretMenuObj.activeSelf) // if already accessing a turret menu at the time of setting a turret, close the menu.
        {
            CloseMenu();
            return;
        }

        turret = _turret;
        SetUInteractive();
        UI.OpenTurretMenu();
    }

    public Turret_Base GetTurret()
    {
        return turret;
    }

    public void SetUITexts(string text1, string text2)
    {
        ammoOne.text = text1;
        ammoTwo.text = text2;

        shieldCost.text = "SHIELD: " + turret.turretDetails.shieldCost;
        upgradeCost.text = "UPGRADE: " + turret.turretDetails.upgradeCost;
        refundCost.text = "REFUND: " + (int)(0.75f * ItemDictionary.instance.shopItems[turret.poolTag].cost);
    }

    private void SetUInteractive()
    {
        if (turret.hasShield)
            shieldButton.interactable = false;
        else
            shieldButton.interactable = true;

        if (string.IsNullOrEmpty(turret.turretDetails.upgradePrefabTag) || !SaveDataManager.instance.ownedItems.Contains(turret.turretDetails.upgradePrefabTag))
            upgradeButton.interactable = false;
        else
            upgradeButton.interactable = true;

        if (turret.turretDetails.ammoTypeOne == turret.turretDetails.ammoTypeTwo) // if there are no difference in ammo types, make ammo buttons inactive
        {
            ammoOneButton.interactable = false;
            ammoTwoButton.interactable = false;
        }
        else if (turret.currentDamageType == turret.turretDetails.ammoTypeOne)
        {
            ammoOneButton.interactable = false;
            ammoTwoButton.interactable = true;
        }
        else if (turret.currentDamageType == turret.turretDetails.ammoTypeTwo)
        {
            ammoTwoButton.interactable = false;
            ammoOneButton.interactable = true;
        }
    }

    public void SetAmmoTypeOne()
    {
        SFXManager2D.instance.PlayStandardItemSelectSFX();
        turret.SetAmmoTypeOne();
        SetUInteractive();
    }

    public void SetAmmoTypeTwo()
    {
        SFXManager2D.instance.PlayStandardItemSelectSFX();
        turret.SetAmmoTypeTwo();
        SetUInteractive();
    }

    public void Upgrade()
    {
        if (PlayerStats.instance.money >= turret.turretDetails.upgradeCost)
        {
            SFXManager2D.instance.PlayPurchaseSFX();
            PlayerStats.instance.RemoveMoney(turret.turretDetails.upgradeCost);
            turret.Upgrade();
            CloseMenu();
        }
        else
        {
            SFXManager2D.instance.PlayErrorSound();
        }
    }

    public void Shield()
    {
        if (PlayerStats.instance.money >= turret.turretDetails.shieldCost)
        {
            SFXManager2D.instance.PlayPurchaseSFX();
            PlayerStats.instance.RemoveMoney(turret.turretDetails.shieldCost);
            turret.ActivateShield();
            SetUInteractive();
        }
        else
        {
            SFXManager2D.instance.PlayErrorSound();
        }
    }

    public void SellTurret()
    {
        float refund = 0.75f * ItemDictionary.instance.shopItems[turret.poolTag].cost;
        PlayerStats.instance.AddMoney((int)refund);
        turret.Refund();
        CloseMenu();
    }

    public void CloseMenu()
    {
        UI.CloseTurretMenu();
    }
}
