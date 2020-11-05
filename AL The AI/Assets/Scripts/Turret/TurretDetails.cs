using UnityEngine;

[System.Serializable]
public class TurretDetails
{
    public string upgradePrefabTag;

    public DamageTypes ammoTypeOne;
    public int ammoOneMinDamage;
    public int ammoOneMaxDamage;
    public DamageTypes ammoTypeTwo;
    public int ammoTwoMinDamage;
    public int ammoTwoMaxDamage;
    public float accuracyOffsetAngle;
    public float fireRate;
    public float turnSpeed;
    public int range;
    public int shieldHealth;
    public int shieldCost;
    public int upgradeCost;
}
