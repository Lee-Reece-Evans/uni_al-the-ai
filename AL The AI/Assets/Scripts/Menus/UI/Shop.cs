using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] private FirstPersonController fpscontroller;
    [SerializeField] private PlayerStats playerStats;

    [Header("Player related")]
    [SerializeField] private GameObject player;
    public bool handsFull = false;

    [Header("Shop Gameobjects")]
    [SerializeField] private GameObject[] selectableItems;
    [SerializeField] private GameObject[] selectableWeapons;

    [Header("Item Text Components")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemModelTxt;
    [SerializeField] private TextMeshProUGUI itemDescTxt;
    [SerializeField] private TextMeshProUGUI itemStatsTxt;
    [SerializeField] private TextMeshProUGUI itemCostTxt;

    //privates
    private GameObject purchasedItem;
    private string itemName;
    private string prefabName;
    private ItemDictionary itemDictionary;
    private WeaponManager weaponManager;

    private void Start()
    {
        itemDictionary = ItemDictionary.instance;
        playerStats = PlayerStats.instance;

        player = GameObject.FindGameObjectWithTag("Player");
        weaponManager = GameObject.Find("Player_Weapons").GetComponent<WeaponManager>();
        fpscontroller = GameObject.Find("FPSController").GetComponent<FirstPersonController>();

        SetItemAvailability();
        SelectItem("Turret1a");
        SetPrefabName("Turret1a_Fake");

        gameObject.SetActive(false);
    }

    public void SetItemAvailability()
    {
        foreach (GameObject item in selectableItems)
        {
            if (!SaveDataManager.instance.ownedItems.Contains(item.name))
            {
                item.transform.Find("LockedText").gameObject.SetActive(true);
                item.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void SelectItem(string _itemName)
    {
        SFXManager2D.instance.PlayStandardItemSelectSFX();

        itemName = _itemName;

        itemModelTxt.text = itemDictionary.shopItems[itemName].model;
        itemDescTxt.text = itemDictionary.shopItems[itemName].description;

        string itemDetails = ValidData("MIN-DAMAGE: ", itemDictionary.shopItems[itemName].minDamage) +
                     ValidData("MAX-DAMAGE: ", itemDictionary.shopItems[itemName].maxDamage) +
                     ValidData("FIRERATE: ", itemDictionary.shopItems[itemName].fireRate) +
                     ValidData("RANGE: ", itemDictionary.shopItems[itemName].range) +
                     ValidData("HEALTH ", itemDictionary.shopItems[itemName].health);

        itemStatsTxt.text = itemDetails;

        itemCostTxt.text = "COST: " + itemDictionary.shopItems[itemName].cost.ToString();
    }

    private string ValidData(string detail, float data)
    {
        if (data != 0)
            return detail + data + "\n";

        return "";
    }

    public void SetPrefabName(string _prefabName)
    {
        prefabName = _prefabName;
    }

    public void PurchaseItem()
    {
        // check for money
        if (itemDictionary.shopItems[itemName].cost > playerStats.money)
        {
            SFXManager2D.instance.PlayErrorSound();
            return;
        }

        // weapons
        if (itemName == "Rifle" || itemName == "Shotgun" || itemName == "Sniper")
        {
            Weapon_Base weapon = weaponManager.weaponScript[int.Parse(prefabName)];

            if (weapon.canUse && weapon.totalAmmo != weapon.maxAmmo) // already own weapon, restock ammo instead
            {
                weapon.AddAmmo();
            }
            else if (weapon.canUse && weapon.totalAmmo == weapon.maxAmmo) // already own weapon and ammo is full
            {
                SFXManager2D.instance.PlayErrorSound();
                return;
            }
            else
            {
                weapon.canUse = true;
                OnScreenUI_Manager.Instance.weaponSlotImages[int.Parse(prefabName)].SetActive(true); // activate weapon slot image

                foreach (GameObject selectableWeapon in selectableWeapons)
                {
                    if (selectableWeapon.name == itemName)
                    {
                        Button weaponButton = selectableWeapon.GetComponent<Button>();
                        weaponButton.transform.Find("AmmoText").gameObject.SetActive(true);
                    }
                }
            }
            playerStats.RemoveMoney(itemDictionary.shopItems[itemName].cost);
        }
        else // placement items
        {
            purchasedItem = ObjectPool.Instance.SpawnFromPool(prefabName);
            if (purchasedItem != null)
            {
                handsFull = true;
                playerStats.RemoveMoney(itemDictionary.shopItems[itemName].cost);

                purchasedItem.transform.position = player.transform.position + player.transform.forward * 4f;
                purchasedItem.transform.rotation = Quaternion.identity;
                purchasedItem.SetActive(true);

                IngameMenuManager.instance.CloseShop();
            }
        }
        SFXManager2D.instance.PlayPurchaseSFX();
    }

    public void CancelPurchase()
    {
        playerStats.AddMoney(itemDictionary.shopItems[itemName].cost);

        Sibling_Placement hasSibling = purchasedItem.GetComponent<Sibling_Placement>();

        if (hasSibling != null && hasSibling.siblingGO != null) // destroy sibling also
        {
            purchasedItem.GetComponent<Sibling_Placement>().siblingGO.transform.parent = null;
            ObjectPool.Instance.ReturnToPool(prefabName, purchasedItem.GetComponent<Sibling_Placement>().siblingGO);
            ObjectPool.Instance.ReturnToPool(prefabName, purchasedItem);
        }
        else
        {
            purchasedItem.transform.parent = null;
            ObjectPool.Instance.ReturnToPool(prefabName, purchasedItem);
        }

        handsFull = false;
    }
}
