using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeShopMenu : MonoBehaviour
{
    // editor assigned
    [SerializeField] private int maxPageNumber;
    [SerializeField] private GameObject[] pages;
    [SerializeField] private GameObject[] items;

    [System.Serializable]
    public struct dependancies
    {
        public string parentName;
        public GameObject dependantObject;
    }
    [SerializeField] private dependancies[] dependants;

    [Header("Components")]
    [SerializeField] private ScrollRect scrollView; // for changing content
    [SerializeField] private GameObject scrollTextObj;
    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("Shop button effect")]
    [SerializeField] private GameObject ShopButtonEffect;

    // interal privates
    private int pageIndex;
    private int previousPageIndex;
    private ItemDictionary itemDictionary;

    private void Start()
    {
        itemDictionary = ItemDictionary.instance;
        CheckShopButtonEffectStatus();
        SetMoneyText();
        previousPageIndex = 0;
        pageIndex = 0;
        prevButton.interactable = false;
        SetItemDetails();
    }

    public void SetItemDetails()
    {
        foreach (GameObject item in items)
        {
            if (SaveDataManager.instance.ownedItems.Contains(item.name)) // check if we own the item already, make sure it can't be purchased again.
            {
                TextMeshProUGUI owned = item.transform.Find("Image").GetComponentInChildren<TextMeshProUGUI>();
                owned.text = "OWNED";
                owned.color = Color.green;

                item.transform.Find("PurchaseButton").GetComponent<Button>().interactable = false;

                // check if the owned item had any dependants.
                for (int i = 0; i < dependants.Length; i++)
                {
                    if (dependants[i].parentName.Equals(item.name) && !SaveDataManager.instance.ownedItems.Contains(dependants[i].dependantObject.name)) // check that the dependant object is not already owned
                    {
                        dependants[i].dependantObject.transform.Find("PurchaseButton").GetComponent<Button>().interactable = true; // allow the purchase of the dependant item.
                    }
                }
            }

            string itemDetails = ValidData("MIN-DAMAGE: ", itemDictionary.shopItems[item.name].minDamage) +
                                 ValidData("MAX-DAMAGE: ", itemDictionary.shopItems[item.name].maxDamage) +
                                 ValidData("FIRERATE: ", itemDictionary.shopItems[item.name].fireRate) +
                                 ValidData("RANGE: ", itemDictionary.shopItems[item.name].range) +
                                 ValidData("HEALTH ", itemDictionary.shopItems[item.name].health);

            item.transform.Find("ItemTitle").GetComponent<TextMeshProUGUI>().text = itemDictionary.shopItems[item.name].model;
            item.transform.Find("ItemDetails").GetComponent<TextMeshProUGUI>().text = itemDictionary.shopItems[item.name].description + "\n" + itemDetails;

            item.transform.Find("PurchaseButton").GetComponentInChildren<TextMeshProUGUI>().text = "PURCHASE " + itemDictionary.shopItems[item.name].unlockCost;
        }
    }

    private string ValidData(string detail, float data)
    {
        if (data != 0)
            return detail + data + "\n";

        return "";
    }

    public void NextButton()
    {
        SFXManager2D.instance.PlayNextSFX();
        prevButton.interactable = true;
        previousPageIndex = pageIndex;
        pageIndex++;

        scrollView.content = pages[pageIndex].GetComponent<RectTransform>();

        if (pageIndex == maxPageNumber)
            nextButton.interactable = false;

        ShowUpgradeMenu();
    }

    public void PreviousButton()
    {
        SFXManager2D.instance.PlayPreviousSFX();
        nextButton.interactable = true;
        previousPageIndex = pageIndex;
        pageIndex--;

        scrollView.content = pages[pageIndex].GetComponent<RectTransform>();

        if (pageIndex == 0)
            prevButton.interactable = false;

        ShowUpgradeMenu();
    }

    private void ShowUpgradeMenu()
    {
        pages[previousPageIndex].SetActive(false); // disable last screen
        pages[pageIndex].SetActive(true); // enable next screen
    }

    public void PurchaseUpgrade(string itemName)
    {
        if (SaveDataManager.instance.money < itemDictionary.shopItems[itemName].unlockCost)
        {
            SFXManager2D.instance.PlayErrorSound();
            return;
        }

        SFXManager2D.instance.PlayPurchaseSFX();
        SaveDataManager.instance.money -= itemDictionary.shopItems[itemName].unlockCost;
        SaveDataManager.instance.ownedItems.Add(itemName);
        SaveDataManager.instance.SaveLevelData();

        SetMoneyText();
        CheckShopButtonEffectStatus();

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].name == itemName)
            {
                TextMeshProUGUI owned = items[i].transform.Find("Image").GetComponentInChildren<TextMeshProUGUI>();
                owned.text = "OWNED";
                owned.color = Color.green;

                items[i].transform.Find("PurchaseButton").GetComponent<Button>().interactable = false;

                // check if the newly owned item had any dependants.
                for (int j = 0; j < dependants.Length; j++)
                {
                    if (dependants[j].parentName.Equals(itemName))
                    {
                        dependants[j].dependantObject.transform.Find("PurchaseButton").GetComponent<Button>().interactable = true; // allow the purchase of the dependant item.
                    }
                }
            }
        }
    }

    private void SetMoneyText()
    {
        moneyText.text = "SHOP: $" + SaveDataManager.instance.money;
    }

    private void CheckShopButtonEffectStatus() // used to indicate the player can affort to buy something
    {
        bool canPurchase = false;
        foreach (GameObject item in items)
        {
            // check if there is an unowned item that can be purchased
            if (!SaveDataManager.instance.ownedItems.Contains(item.name) &&
                SaveDataManager.instance.money >= itemDictionary.shopItems[item.name].unlockCost)
            {
                canPurchase = true;
                ShopButtonEffect.SetActive(true);
                break;
            }
        }

        if (!canPurchase)
            ShopButtonEffect.SetActive(false);
    }
    public void ToggleScrollText()
    {
        if (scrollBar.value <= 0.2 && scrollTextObj.activeSelf)
            scrollTextObj.SetActive(false);
        else if (scrollBar.value > 0.2 &&!scrollTextObj.activeSelf)
            scrollTextObj.SetActive(true);
    }
}
