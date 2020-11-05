using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    public static ItemDictionary instance;

    public Dictionary<string, ShopItemDetails> shopItems = new Dictionary<string, ShopItemDetails>();

    public struct ShopItemDetails
    {
        public string model;
        public string description;
        public int minDamage;
        public int maxDamage;
        public float fireRate;
        public int range;
        public int health;
        public int cost;
        public int unlockCost;
    }

    private void Awake() // persistant singleton
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            PopulateItemInfo();
        }
    }

    private void PopulateItemInfo() // change to loop through array of prefabs in the shop
    {
        ShopItemDetails item = new ShopItemDetails
        {
            model = "Gun Turret - MK1",
            description = "Can be set to shoot anti-armor or shield-piercing rounds. Deals x2 damage to enemies with weakness to ammotype.",
            minDamage = 2,
            maxDamage = 4,
            fireRate = 1,
            range = 10,
            health = 200,
            cost = 150,
            unlockCost = 0
        };
        shopItems.Add("Turret1a", item);

        item = new ShopItemDetails
        {
            model = "Gun Turret - MK2",
            description = "(Requires Gun Turret MK1) Improved Gun Turret.",
            minDamage = 3,
            maxDamage = 6,
            fireRate = 1,
            range = 10,
            health = 275,
            cost = 200,
            unlockCost = 500
        };
        shopItems.Add("Turret1b", item);

        item = new ShopItemDetails
        {
            model = "Rocket Turret - MK1",
            description = "Detector - reveals invisible enemies allowing turrets to target and shoot at them.",
            minDamage = 1,
            maxDamage = 2,
            fireRate = 2.25f,
            range = 10,
            health = 225,
            cost = 200,
            unlockCost = 500
        };
        shopItems.Add("Turret2a", item);

        item = new ShopItemDetails
        {
            model = "Rocket Turret - MK2",
            description = "(Requires Rocket Turret MK1) - Improved detector turret.",
            minDamage = 1,
            maxDamage = 2,
            fireRate = 2.25f,
            range = 10,
            health = 300,
            cost = 250,
            unlockCost = 500
        };
        shopItems.Add("Turret2b", item);

        item = new ShopItemDetails
        {
            model = "Flame Turret - MK1",
            description = "Slow firing turret, can be set to deal area damage or single target focus with high damage (125 - 150).",
            minDamage = 6,
            maxDamage = 10,
            fireRate = 5,
            range = 20,
            health = 175,
            cost = 250,
            unlockCost = 500
        };
        shopItems.Add("Turret3a", item);

        item = new ShopItemDetails
        {
            model = "Flame Turret - MK2",
            description = "(Requires Flame Turret MK1) Improved Flame Turret",
            minDamage = 8,
            maxDamage = 13,
            fireRate = 1,
            range = 7,
            health = 250,
            cost = 300,
            unlockCost = 500
        };
        shopItems.Add("Turret3b", item);

        item = new ShopItemDetails
        {
            model = "Freeze Trap",
            description = "Slows enemies speed by half when entering its range.",
            minDamage = 0,
            maxDamage = 0,
            fireRate = 0,
            range = 5,
            health = 0,
            cost = 200,
            unlockCost = 500
        };
        shopItems.Add("Freeze_Trap", item);

        item = new ShopItemDetails
        {
            model = "Repair Drone",
            description = "Repairs damanged structures automatically, can wander an area or be ordered to hold position.",
            minDamage = 0,
            maxDamage = 0,
            fireRate = 0,
            range = 0,
            health = 0,
            cost = 250,
            unlockCost = 500
        };
        shopItems.Add("Repair_Drone", item);

        item = new ShopItemDetails
        {
            model = "Mine Explosive",
            description = "Explosive device that deals area damage when stepped on.",
            minDamage = 7,
            maxDamage = 12,
            fireRate = 0,
            range = 5,
            health = 0,
            cost = 50,
            unlockCost = 500
        };
        shopItems.Add("Mine", item);

        item = new ShopItemDetails
        {
            model = "Portal",
            description = "Allows fast travel between 2 points.",
            minDamage = 0,
            maxDamage = 0,
            fireRate = 0,
            range = 0,
            health = 0,
            cost = 150,
            unlockCost = 500
        };
        shopItems.Add("Portal", item);

        item = new ShopItemDetails
        {
            model = "Barrier",
            description = "High health barrier stops enemies from passing through.",
            minDamage = 0,
            maxDamage = 0,
            fireRate = 0,
            range = 0,
            health = 750,
            cost = 300,
            unlockCost = 500
        };
        shopItems.Add("Barrier", item);

        item = new ShopItemDetails
        {
            model = "Assault Rifle",
            description = "Automatic Rifle = ClipSize = 25; Max Ammo = 240. Deals half damage to shield enemies.",
            minDamage = 7,
            maxDamage = 9,
            fireRate = 1.25f,
            range = 0,
            health = 0,
            cost = 200,
            unlockCost = 0
        };
        shopItems.Add("Rifle", item);

        item = new ShopItemDetails
        {
            model = "Shotgun",
            description = "Automatic Shotgun = Shoots 5 bullets at once; ClipSize = 6; Max Ammo = 54. Deals half damage to shield enemies.",
            minDamage = 7,
            maxDamage = 9,
            fireRate = 1,
            range = 0,
            health = 0,
            cost = 300,
            unlockCost = 0
        };
        shopItems.Add("Shotgun", item);

        item = new ShopItemDetails
        {
            model = "Sniper Rifle",
            description = "Sniper Rifle = ClipSize = 1; Max Ammo = 30. Shield Piercing - Deals double damage to shield enemies.",
            minDamage = 75,
            maxDamage = 100,
            fireRate = 1,
            range = 0,
            health = 0,
            cost = 350,
            unlockCost = 0
        };
        shopItems.Add("Sniper", item);
    }
}
