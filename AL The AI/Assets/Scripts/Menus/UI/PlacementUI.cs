using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlacementUI : MonoBehaviour
{
    public TextMeshProUGUI refundCost;

    private Structure_Placement structure;
    private IngameMenuManager UI;

    public void SetStructure(Structure_Placement _structure)
    {
        if (UI == null)
            UI = IngameMenuManager.instance;

        if (UI.placementMenuObj.activeSelf)
        {
            CloseMenu();
            return;
        }

        structure = _structure;

        refundCost.text = "REFUND: " + ItemDictionary.instance.shopItems[structure.structurePoolTag].cost;

        UI.OpenPlacementMenu();
    }

    public void RefundStructure()
    {
        int refund = ItemDictionary.instance.shopItems[structure.structurePoolTag].cost;
        PlayerStats.instance.AddMoney(refund);
        structure.Refund();
        UI.ClosePlacementMenu();
    }

    public void CloseMenu()
    {
        UI.ClosePlacementMenu();
    }
}
