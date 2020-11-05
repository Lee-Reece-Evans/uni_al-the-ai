using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RepairDroneUI : MonoBehaviour
{
    public TextMeshProUGUI refundCost;

    private drone drone;
    private IngameMenuManager UI;

    public void SetDrone(drone _drone)
    {
        if (UI == null)
            UI = IngameMenuManager.instance;

        if (UI.repairDroneMenuObj.activeSelf) // if at the time of interaction i already have a menu open, close it.
        {
            CloseMenu();
            return;
        }

        drone = _drone;

        SetUITexts();
        UI.OpenRepairDroneMenu();
    }

    public drone GetDrone()
    {
        return drone;
    }

    public void SetUITexts()
    {
        refundCost.text = "REFUND: " + (int)(0.75f * ItemDictionary.instance.shopItems[drone.poolTag].cost);
    }

    public void SellRepairDrone()
    {
        float refund = 0.75f * ItemDictionary.instance.shopItems[drone.poolTag].cost;
        PlayerStats.instance.AddMoney((int)refund);
        drone.Refund();
        CloseMenu();
    }

    public void EnableWander()
    {
        SFXManager2D.instance.PlayStandardItemSelectSFX();
        drone.EnableWander();
        CloseMenu();
    }

    public void EnableHoldPosition()
    {
        SFXManager2D.instance.PlayStandardItemSelectSFX();
        drone.EnableHoldPosition();
        CloseMenu();
    }

    public void CloseMenu()
    {
        UI.CloseRepairDroneMenu();
    }
}
